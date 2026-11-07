using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Replaces native imports with typed delegates, lazy export caches, and targeted ABI bridges.
/// </summary>
sealed class LegacyBindings : CSharpSyntaxRewriter
{
    /// <summary>Propagates compiler cancellation through declaration traversal.</summary>
    private readonly CancellationToken cancellationToken;

    /// <summary>Creates a binding rewriter for the current compiler operation.</summary>
    internal LegacyBindings(CancellationToken cancellationToken = default)
    {
        this.cancellationToken = cancellationToken;
    }

    /// <summary>Checks cancellation before traversing each declaration node.</summary>
    public override SyntaxNode? Visit(SyntaxNode? node)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return base.Visit(node);
    }

    /// <summary>
    /// Gets the number of imports rewritten in the current source file.
    /// </summary>
    public int Count
    {
        get;
        private set;
    }

    /// <summary>
    /// Builds a deferred delegate cache whose failed symbol lookup can be retried.
    /// </summary>
    private static MemberDeclarationSyntax CreateCache(string name, string delegateType, string library, string entryPoint) =>
        SyntaxFactory.ParseMemberDeclaration($$"""
            /// <summary>Retains the native delegate after its first successful lookup.</summary>
            private static class {{name}}
            {
                /// <summary>Prevents the runtime from resolving this binding before its first use.</summary>
                static {{name}}() { }

                /// <summary>Stores the resolved delegate; failed lookups leave it null.</summary>
                private static {{delegateType}} function;

                // A failed lookup leaves the cache empty so corrected configuration can be retried.
                /// <summary>Gets the cached delegate, retrying resolution after a previous lookup failure.</summary>
                internal static {{delegateType}} Function => System.Threading.LazyInitializer.EnsureInitialized(
                    ref function, () => NativeDependencies.GetFunction<{{delegateType}}>({{library}}, {{entryPoint}}));
            }
            """)!;

    /// <summary>
    /// Preserves field metadata while replacing the Windows C long storage type with a 32-bit integer.
    /// </summary>
    private static string WindowsField(FieldDeclarationSyntax field)
    {
        var type = field.Declaration.Type;

        if (type.ToString() == "CLong")
        {
            type = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)).WithTriviaFrom(type);
        }

        return field.WithDeclaration(field.Declaration.WithType(type)).ToFullString();
    }

    /// <summary>
    /// Rewrites imports within a class while preserving shared API signatures, documentation, and conditional directives.
    /// </summary>
    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var members = new List<MemberDeclarationSyntax>();

        foreach (var member in node.Members)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (member is not MethodDeclarationSyntax method)
            {
                members.Add((MemberDeclarationSyntax)Visit(member)!);

                if (member is StructDeclarationSyntax structure && structure.Identifier.ValueText == "cache_config_t")
                {
                    var fields = structure.Members.OfType<FieldDeclarationSyntax>().ToArray();
                    // Copy field documentation while substituting only the platform-dependent C long type.
                    string nativeFields = string.Join("\n", fields.Select(WindowsField));
                    // Fixed buffers require an element copy; scalar fields can use an object initializer.
                    var plainFields = fields.Where(f => !f.Modifiers.Any(SyntaxKind.FixedKeyword));
                    string toNative = string.Join(",", plainFields.SelectMany(f => f.Declaration.Variables).Select(v => v.Identifier.ValueText)
                        .Select(n => $"{n} = " + (n == "epoch_length" ? "checked((int)value.epoch_length.Value)" : $"value.{n}")));
                    string toManaged = string.Join(",", plainFields.SelectMany(f => f.Declaration.Variables).Select(v => v.Identifier.ValueText)
                        .Select(n => $"{n} = " + (n == "epoch_length" ? "new CLong(value.epoch_length)" : "value." + n)));
                    // Include the terminator slot declared by the native fixed-size filename buffer.
                    string copyBuffer = "for (int i = 0; i <= MAX_TRACE_FILE_NAME_LEN; i++) result.trace_file_name[i] = value.trace_file_name[i]; return result;";
                    // Windows C long is 32-bit even in a 64-bit process.
                    members.Add(SyntaxFactory.ParseMemberDeclaration($$"""
                        /// <summary>Matches the Windows cache configuration ABI with a 32-bit C long field.</summary>
                        internal struct WindowsCacheConfig
                        {
                            {{nativeFields}}

                            /// <summary>Converts managed configuration to Windows layout with checked narrowing.</summary>
                            internal static WindowsCacheConfig FromManaged(cache_config_t value)
                            {
                                var result = new WindowsCacheConfig { {{toNative}} };
                                {{copyBuffer}}
                            }

                            /// <summary>Copies native output into the shared managed configuration layout.</summary>
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

            var imports = method.AttributeLists.SelectMany(a => a.Attributes)
                .Where(a => a.Name.ToString() == "DllImport").ToArray();

            if (imports.Length > 1)
            {
                throw new BindingGenerationException("Expected one DllImport attribute per declaration.", method.GetLocation());
            }

            var import = imports.SingleOrDefault();

            if (import == null)
            {
                members.Add(method);
                continue;
            }

            // Only native declarations may lose their extern modifier and acquire a generated body.
            if (!method.Modifiers.Any(SyntaxKind.StaticKeyword) || !method.Modifiers.Any(SyntaxKind.ExternKeyword) ||
                method.TypeParameterList != null || method.Body != null || method.ExpressionBody != null)
            {
                throw new BindingGenerationException("Expected a static, non-generic extern import without a method body.", method.GetLocation());
            }
            if (import.ArgumentList == null || import.ArgumentList.Arguments.Count == 0)
            {
                throw new BindingGenerationException("Missing import library.", method.GetLocation());
            }

            var arguments = import.ArgumentList.Arguments;
            string library = arguments[0].Expression.ToString();

            if (library is not ("Constants.DLLFileName" or "Constants.HLDLLFileName"))
            {
                throw new BindingGenerationException($"Unexpected import library: {library}", method.GetLocation());
            }

            var named = new Dictionary<string, string>();

            foreach (var argument in arguments.Skip(1))
            {
                string? name = argument.NameEquals?.Name.Identifier.ValueText;

                if (name == null || named.ContainsKey(name))
                {
                    throw new BindingGenerationException("Expected unique named DllImport options after the library argument.", argument.GetLocation());
                }

                named.Add(name, argument.Expression.ToString());
            }

            if (!named.TryGetValue("EntryPoint", out string? entryPoint))
            {
                throw new BindingGenerationException("Missing explicit entry point.", method.GetLocation());
            }

            named.Remove("EntryPoint");

            if (!named.TryGetValue("CallingConvention", out string? convention))
            {
                throw new BindingGenerationException("Missing calling convention.", method.GetLocation());
            }

            named.Remove("CallingConvention");

            // Reject new import options until their delegate-marshalling equivalents are understood.
            var supported = new[] { "CharSet", "SetLastError", "BestFitMapping", "ThrowOnUnmappableChar" };

            if (named.Keys.Any(k => !supported.Contains(k)))
            {
                throw new BindingGenerationException("Unsupported DllImport option.", method.GetLocation());
            }

            string pointerOptions = string.Join("", named.Select(p => $", {p.Key} = {p.Value}"));
            string suffix = "__Legacy" + Count++;
            string returnAttributes = string.Join("\n", method.AttributeLists.Where(a => a.Target?.Identifier.ValueText == "return"));
            // Delegate parameter attributes retain all native marshalling semantics.
            var parameters = method.ParameterList.WithParameters(SyntaxFactory.SeparatedList(
                method.ParameterList.Parameters.Select(p => p.WithDefault(null))));
            string declaration = $"/// <summary>Calls the native export {entryPoint.Trim('"')} with the shared declaration's ABI.</summary>\n[UnmanagedFunctionPointer({convention}{pointerOptions})] {returnAttributes} private delegate {method.ReturnType} {suffix}Delegate{parameters};";

            if (entryPoint != "\"H5Ewalk2\"")
            {
                members.Add(SyntaxFactory.ParseMemberDeclaration(declaration)!);
            }

            // Lazy caches avoid loading HDF5 before Configure or dont_atexit can run.
            if (entryPoint != "\"H5Ewalk2\"")
            {
                members.Add(CreateCache(suffix + "Cache", suffix + "Delegate", library, entryPoint));
            }

            // Default values belong on the public API; ref/out/in modifiers must also survive forwarding.
            string actuals = string.Join(", ", method.ParameterList.Parameters.Select(p =>
                string.Join(" ", p.Modifiers.Where(m => m.IsKind(SyntaxKind.RefKeyword) || m.IsKind(SyntaxKind.OutKeyword) || m.IsKind(SyntaxKind.InKeyword)).Select(m => m.Text).Append(p.Identifier.Text))));
            var lists = method.AttributeLists.Select(list => list.WithAttributes(SyntaxFactory.SeparatedList(list.Attributes.Where(a => a != import))))
                .Where(list => list.Attributes.Count != 0);
            var body = SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ParseExpression($"{suffix}Cache.Function({actuals})"));
            var implementation = method.WithAttributeLists(SyntaxFactory.List(lists))
                .WithModifiers(SyntaxFactory.TokenList(method.Modifiers.Where(m => !m.IsKind(SyntaxKind.ExternKeyword))))
                .WithExpressionBody(body).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            // Keep export metadata available to validation even though DllImport itself is removed.
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
                // A getter consumes only version and must not overwrite caller state when native code fails.
                string conversion = entryPoint == "\"H5Fget_mdc_config\"" ? $"new H5AC.WindowsCacheConfig {{ version = {name}.version }}" : $"H5AC.WindowsCacheConfig.FromManaged({name})";
                string copyBack = entryPoint == "\"H5Fget_mdc_config\"" ? $"if (result >= 0) {name} = native.ToManaged();" : string.Empty;
                string code = $"{{ if (!LegacyNativeLibrary.IsWindows) return {suffix}Cache.Function({actuals}); var native = {conversion}; var result = {suffix}WindowsCache.Function({windowsArgs}); {copyBack} return result; }}";
                implementation = implementation.WithExpressionBody(null).WithSemicolonToken(default)
                    .WithBody((BlockSyntax)SyntaxFactory.ParseStatement(code));
            }

            // Error records contain borrowed strings that require the handwritten pointer-layout bridge.
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
