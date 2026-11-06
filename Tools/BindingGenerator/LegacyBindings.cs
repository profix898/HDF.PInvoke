using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class LegacyBindings : CSharpSyntaxRewriter
{
    public int Count
    {
        get;
        private set;
    }

    private static MemberDeclarationSyntax CreateCache(string name, string delegateType, string library, string entryPoint) =>
        SyntaxFactory.ParseMemberDeclaration($$"""
            private static class {{name}}
            {
                static {{name}}() { }
                private static {{delegateType}} function;

                // A failed lookup leaves the cache empty so corrected configuration can be retried.
                internal static {{delegateType}} Function => System.Threading.LazyInitializer.EnsureInitialized(
                    ref function, () => NativeDependencies.GetFunction<{{delegateType}}>({{library}}, {{entryPoint}}));
            }
            """)!;

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var members = new List<MemberDeclarationSyntax>();

        foreach (var member in node.Members)
        {
            if (member is not MethodDeclarationSyntax method)
            {
                members.Add((MemberDeclarationSyntax)Visit(member)!);

                if (member is StructDeclarationSyntax structure && structure.Identifier.ValueText == "cache_config_t")
                {
                    var fields = structure.Members.OfType<FieldDeclarationSyntax>().ToArray();
                    string nativeFields = string.Join("\n", fields.Select(f => f.WithoutTrivia().ToString().Replace("CLong", "int")));
                    var plainFields = fields.Where(f => !f.Modifiers.Any(SyntaxKind.FixedKeyword));
                    string toNative = string.Join(",", plainFields.SelectMany(f => f.Declaration.Variables).Select(v => v.Identifier.ValueText)
                        .Select(n => $"{n} = " + (n == "epoch_length" ? "checked((int)value.epoch_length.Value)" : $"value.{n}")));
                    string toManaged = string.Join(",", plainFields.SelectMany(f => f.Declaration.Variables).Select(v => v.Identifier.ValueText)
                        .Select(n => $"{n} = " + (n == "epoch_length" ? "new CLong(value.epoch_length)" : "value." + n)));
                    string copyBuffer = "for (int i = 0; i <= MAX_TRACE_FILE_NAME_LEN; i++) result.trace_file_name[i] = value.trace_file_name[i]; return result;";
                    // Windows C long is 32-bit even in a 64-bit process.
                    members.Add(SyntaxFactory.ParseMemberDeclaration($$"""
                        internal struct WindowsCacheConfig
                        {
                            {{nativeFields}}

                            internal static WindowsCacheConfig FromManaged(cache_config_t value)
                            {
                                var result = new WindowsCacheConfig { {{toNative}} };
                                {{copyBuffer}}
                            }

                            internal cache_config_t ToManaged()
                            {
                                var value = this;
                                var result = new cache_config_t { {{toManaged}} };
                                {{copyBuffer}}
                            }
                        }
                        """)!);
                }

                continue;
            }

            var import = method.AttributeLists.SelectMany(a => a.Attributes)
                .SingleOrDefault(a => a.Name.ToString() == "DllImport");

            if (import == null)
            {
                members.Add(method);
                continue;
            }

            var arguments = import.ArgumentList!.Arguments;
            string library = arguments[0].Expression.ToString();

            if (library is not ("Constants.DLLFileName" or "Constants.HLDLLFileName"))
            {
                throw new InvalidOperationException($"Unexpected import library: {library}");
            }

            var named = arguments.Skip(1).ToDictionary(a => a.NameEquals!.Name.Identifier.ValueText, a => a.Expression.ToString());

            if (!named.Remove("EntryPoint", out string? entryPoint))
            {
                throw new InvalidOperationException("Missing explicit entry point.");
            }

            if (!named.Remove("CallingConvention", out string? convention))
            {
                throw new InvalidOperationException("Missing calling convention.");
            }

            var supported = new[] { "CharSet", "SetLastError", "BestFitMapping", "ThrowOnUnmappableChar" };

            if (named.Keys.Any(k => !supported.Contains(k)))
            {
                throw new InvalidOperationException("Unsupported DllImport option.");
            }

            string pointerOptions = string.Join("", named.Select(p => $", {p.Key} = {p.Value}"));
            string suffix = "__Legacy" + Count++;
            string returnAttributes = string.Join("\n", method.AttributeLists.Where(a => a.Target?.Identifier.ValueText == "return"));
            // Delegate parameter attributes retain all native marshalling semantics.
            var parameters = method.ParameterList.WithParameters(SyntaxFactory.SeparatedList(
                method.ParameterList.Parameters.Select(p => p.WithDefault(null))));
            string declaration = $"[UnmanagedFunctionPointer({convention}{pointerOptions})] {returnAttributes} private delegate {method.ReturnType} {suffix}Delegate{parameters};";

            if (entryPoint != "\"H5Ewalk2\"")
            {
                members.Add(SyntaxFactory.ParseMemberDeclaration(declaration)!);
            }

            // Lazy caches avoid loading HDF5 before Configure or dont_atexit can run.
            if (entryPoint != "\"H5Ewalk2\"")
            {
                members.Add(CreateCache(suffix + "Cache", suffix + "Delegate", library, entryPoint));
            }

            string actuals = string.Join(", ", method.ParameterList.Parameters.Select(p =>
                string.Join(" ", p.Modifiers.Where(m => m.IsKind(SyntaxKind.RefKeyword) || m.IsKind(SyntaxKind.OutKeyword) || m.IsKind(SyntaxKind.InKeyword)).Select(m => m.Text).Append(p.Identifier.Text))));
            var lists = method.AttributeLists.Select(list => list.WithAttributes(SyntaxFactory.SeparatedList(list.Attributes.Where(a => a != import))))
                .Where(list => list.Attributes.Count != 0);
            var body = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"{suffix}Cache.Function({actuals})"));
            var implementation = method.WithAttributeLists(SyntaxFactory.List(lists))
                .WithModifiers(SyntaxFactory.TokenList(method.Modifiers.Where(m => !m.IsKind(SyntaxKind.ExternKeyword))))
                .WithExpressionBody(body).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            implementation = implementation.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Attribute(SyntaxFactory.ParseName("LegacyImport"), SyntaxFactory.ParseAttributeArgumentList($"({library}, {entryPoint})")))));
            var config = method.ParameterList.Parameters.SingleOrDefault(p => p.Type!.ToString() == "H5AC.cache_config_t");

            if (config != null)
            {
                string windowsDeclaration = declaration.Replace(suffix + "Delegate", suffix + "WindowsDelegate")
                                                    .Replace("H5AC.cache_config_t", "H5AC.WindowsCacheConfig");
                members.Add(SyntaxFactory.ParseMemberDeclaration(windowsDeclaration)!);
                members.Add(CreateCache(suffix + "WindowsCache", suffix + "WindowsDelegate", library, entryPoint));
                string name = config.Identifier.Text;
                string windowsArgs = actuals.Replace("ref " + name, "ref native");
                string conversion = entryPoint == "\"H5Fget_mdc_config\"" ? $"new H5AC.WindowsCacheConfig {{ version = {name}.version }}" : $"H5AC.WindowsCacheConfig.FromManaged({name})";
                string copyBack = entryPoint == "\"H5Fget_mdc_config\"" ? $"if (result >= 0) {name} = native.ToManaged();" : string.Empty;
                string code = $"{{ if (!LegacyNativeLibrary.IsWindows) return {suffix}Cache.Function({actuals}); var native = {conversion}; var result = {suffix}WindowsCache.Function({windowsArgs}); {copyBack} return result; }}";
                implementation = implementation.WithExpressionBody(null).WithSemicolonToken(default)
                    .WithBody((BlockSyntax)SyntaxFactory.ParseStatement(code));
            }

            if (entryPoint == "\"H5Ewalk2\"")
            {
                implementation = implementation.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(
                                                                    SyntaxFactory.ParseExpression($"LegacyErrorWalk.Invoke({actuals})")));
            }

            // Removing an attribute list must not discard the method's XML docs or directives.
            members.Add(implementation.WithLeadingTrivia(method.GetLeadingTrivia()));
        }

        return node.WithMembers(SyntaxFactory.List(members));
    }
}
