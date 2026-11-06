using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class GeneratorTests
{
    internal static void Run()
    {
        string declaration = "[DllImport(Constants.DLLFileName, EntryPoint = \"example\", CallingConvention = CallingConvention.Cdecl)] public static extern int Call([MarshalAs(UnmanagedType.LPUTF8Str)] string text, ref int value);";
        string output = BindingSource.Generate("class H5 { " + declaration + " }", out int count);
        Require(count == 1 && !output.Contains("DllImport("), "Replace imports exactly once");
        Require(output.Contains("static __Legacy0Cache()") && output.Contains("ref value"), "Preserve lazy initialization and ref parameters");
        Require(output.Contains("typeof(Utf8Marshaler)") && !output.Contains("MarshalCookie"), "Own forward-call string allocations");

        Require(output.Contains("LazyInitializer.EnsureInitialized"), "Keep failed lookups retryable");

        BindingSource.Generate("class H5 {\n#if CUSTOM\n" + declaration + "\n#endif\n}", out int conditionalCount, new[] { "CUSTOM" });
        Require(conditionalCount == 1, "Honor the compilation's preprocessor symbols");

        string documented = BindingSource.Generate("class H5 {\n/// <summary>Example</summary>\n" + declaration + "\n}", out _);
        var documentedMethod = CSharpSyntaxTree.ParseText(documented).GetRoot().DescendantNodes()
            .OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.ValueText == "Call");
        Require(documentedMethod.GetLeadingTrivia().ToFullString().Contains("Example"), "Preserve import XML documentation");

        string callback = "class H5 { public delegate int Callback([MarshalAs(UnmanagedType.LPUTF8Str)] string text); }";
        Require(BindingSource.Generate(callback, out _).Contains("MarshalCookie = \"borrowed\""), "Borrow callback strings");

        string literal = "class H5 { public const string Text = \"UnmanagedType.LPUTF8Str\"; }";
        Require(BindingSource.Generate(literal, out _).Contains("\"UnmanagedType.LPUTF8Str\""), "Preserve string literals");

        ExpectFailure("class H5 { " + declaration.Replace("Constants.DLLFileName", "\"other\"") + " }", "Reject unknown libraries");
        ExpectFailure("class H5 { " + declaration.Replace("CallingConvention = CallingConvention.Cdecl", "CallingConvention = CallingConvention.Cdecl, PreserveSig = false") + " }", "Reject unsupported import options");
        ExpectFailure("class H5 { struct Unknown { [MarshalAs(UnmanagedType.LPUTF8Str)] public string value; } }", "Reject unbridged UTF-8 fields");

        ExpectFailure("class H5 { public delegate string Callback(); }", "Reject unbridged string results");
        ExpectFailure("class H5 { public delegate int Callback(ref string text); }", "Reject unbridged string outputs");

        string getter = "class H5F { [DllImport(Constants.DLLFileName, EntryPoint = \"H5Fget_mdc_config\", CallingConvention = CallingConvention.Cdecl)] public static extern int Get(int file, ref H5AC.cache_config_t config); }";
        output = BindingSource.Generate(getter, out _);
        Require(!output.Contains("FromManaged(config)") && output.Contains("version = config.version") && output.Contains("if (result >= 0)"), "Preserve getter input and failure semantics");

        string bridge = "class H5E { [DllImport(Constants.DLLFileName, EntryPoint = \"H5Ewalk2\", CallingConvention = CallingConvention.Cdecl)] public static extern int Walk(int value); }";
        output = BindingSource.Generate(bridge, out _);
        Require(output.Contains("LegacyErrorWalk.Invoke") && !output.Contains("__Legacy0Delegate"), "Avoid unused bridge delegates");

        // Read the generated declaration to check parameter attributes independently of text formatting.
        var method = CSharpSyntaxTree.ParseText(BindingSource.Generate("class H5 { " + declaration + " }", out _)).GetRoot()
            .DescendantNodes().OfType<DelegateDeclarationSyntax>().Single();
        Require(method.ParameterList.Parameters[1].Modifiers.ToString() == "ref", "Preserve delegate parameter modifiers");
        Console.WriteLine("PASS: 16 binding-generator regression checks");
    }

    private static void ExpectFailure(string source, string description)
    {
        try
        {
            BindingSource.Generate(source, out _);
        }
        catch (InvalidOperationException)
        {
            return;
        }

        throw new InvalidOperationException(description);
    }

    private static void Require(bool condition, string description)
    {
        if (!condition)
        {
            throw new InvalidOperationException(description);
        }
    }
}
