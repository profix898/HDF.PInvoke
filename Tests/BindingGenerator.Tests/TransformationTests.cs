using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Checks representative syntax, ABI, and ownership transformations without loading HDF5.
/// </summary>
[TestClass]
public sealed class TransformationTests
{
    /// <summary>Disambiguates private caches in partial declarations without depending on checkout paths.</summary>
    [TestMethod]
    public void PartialClassCachesHaveStableDistinctNames()
    {
        const string first = "partial class H5 { [DllImport(Constants.DLLFileName, EntryPoint = \"first\", CallingConvention = CallingConvention.Cdecl)] public static extern int First(); }";
        var second = first.Replace("first", "second").Replace("First", "Second");
        var a = BindingSource.Generate(first, out _, path: "/one/First.cs");
        var b = BindingSource.Generate(second, out _, path: "/two/Second.cs");
        var namesA = CSharpSyntaxTree.ParseText(a).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Select(n => n.Identifier.ValueText).Where(n => n.StartsWith("__Legacy")).ToArray();
        var namesB = CSharpSyntaxTree.ParseText(b).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Select(n => n.Identifier.ValueText).Where(n => n.StartsWith("__Legacy")).ToArray();
        Assert.AreEqual(1, namesA.Length);
        Assert.AreEqual(1, namesB.Length);
        Assert.AreNotEqual(namesA[0], namesB[0]);
        Assert.AreEqual(a, BindingSource.Generate(first, out _, path: "/different/First.cs"));
    }

    /// <summary>
    /// Preserves the established signature, ownership, metadata, and ABI conversion contracts.
    /// </summary>
    [TestMethod]
    public void PreservesSharedBindingContracts()
    {
        var declaration = "[DllImport(Constants.DLLFileName, EntryPoint = \"example\", CallingConvention = CallingConvention.Cdecl)] public static extern int Call([MarshalAs(UnmanagedType.LPUTF8Str)] string text, ref int value);";
        var output = BindingSource.Generate("class H5 { " + declaration + " }", out var count);
        Require(count == 1 && !output.Contains("DllImport("), "Replace imports exactly once");
        Require(output.Contains("Cache()") && output.Contains("ref value"), "Preserve lazy initialization and ref parameters");
        Require(output.Contains("typeof(UTF8Marshaler)") && !output.Contains("MarshalCookie"), "Own forward-call string allocations");

        Require(output.Contains("LazyInitializer.EnsureInitialized"), "Keep failed lookups retryable");

        BindingSource.Generate("class H5 {\n#if CUSTOM\n" + declaration + "\n#endif\n}", out var conditionalCount, new[] { "CUSTOM" });
        Require(conditionalCount == 1, "Honor the compilation's preprocessor symbols");

        var documented = BindingSource.Generate("class H5 {\n/// <summary>Example</summary>\n" + declaration + "\n}", out _);
        var documentedMethod = CSharpSyntaxTree.ParseText(documented).GetRoot().DescendantNodes()
            .OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.ValueText == "Call");
        Require(documentedMethod.GetLeadingTrivia().ToFullString().Contains("Example"), "Preserve import XML documentation");

        var callback = "class H5 { public delegate int Callback([MarshalAs(UnmanagedType.LPUTF8Str)] string text); }";
        Require(BindingSource.Generate(callback, out _).Contains("MarshalCookie = \"borrowed\""), "Borrow callback strings");

        var literal = "class H5 { public const string Text = \"UnmanagedType.LPUTF8Str\"; }";
        Require(BindingSource.Generate(literal, out _).Contains("\"UnmanagedType.LPUTF8Str\""), "Preserve string literals");

        ExpectFailure("class H5 { " + declaration.Replace("Constants.DLLFileName", "\"other\"") + " }", "Reject unknown libraries");
        ExpectFailure("class H5 { " + declaration.Replace("CallingConvention = CallingConvention.Cdecl", "CallingConvention = CallingConvention.Cdecl, PreserveSig = false") + " }", "Reject unsupported import options");
        ExpectFailure("class H5 { struct Unknown { [MarshalAs(UnmanagedType.LPUTF8Str)] public string value; } }", "Reject unbridged UTF-8 fields");

        ExpectFailure("class H5 { public delegate string Callback(); }", "Reject unbridged string results");
        ExpectFailure("class H5 { public delegate int Callback(ref string text); }", "Reject unbridged string outputs");

        var getter = "class H5F { [DllImport(Constants.DLLFileName, EntryPoint = \"H5Fget_mdc_config\", CallingConvention = CallingConvention.Cdecl)] public static extern int Get(int file, ref H5AC.cache_config_t config); }";
        output = BindingSource.Generate(getter, out _);
        Require(!output.Contains("FromManaged(config)") && output.Contains("version = config.version") && output.Contains("if (result >= 0)"), "Preserve getter input and failure semantics");

        var bridge = "class H5E { [DllImport(Constants.DLLFileName, EntryPoint = \"H5Ewalk2\", CallingConvention = CallingConvention.Cdecl)] public static extern int Walk(int value); }";
        output = BindingSource.Generate(bridge, out _);
        Require(output.Contains("LegacyErrorWalk.Invoke") && !output.Contains("Delegate("), "Avoid unused bridge delegates");

        // Read the generated declaration to check parameter attributes independently of text formatting.
        var method = CSharpSyntaxTree.ParseText(BindingSource.Generate("class H5 { " + declaration + " }", out _)).GetRoot()
            .DescendantNodes().OfType<DelegateDeclarationSyntax>().Single();
        Require(method.ParameterList.Parameters[1].Modifiers.ToString() == "ref", "Preserve delegate parameter modifiers");
        Require(method.GetLeadingTrivia().ToFullString().Contains("<summary>"), "Document generated native delegates");

        var layout = "class H5AC { public struct cache_config_t {\n/// <summary>A CLong value.</summary>\npublic CLong epoch_length; } }";
        var generatedLayout = CSharpSyntaxTree.ParseText(BindingSource.Generate(layout, out _)).GetRoot()
            .DescendantNodes().OfType<StructDeclarationSyntax>().Single(s => s.Identifier.ValueText == "WindowsCacheConfig");
        var field = generatedLayout.Members.OfType<FieldDeclarationSyntax>().Single();
        Require(field.Declaration.Type.ToString() == "int" && field.GetLeadingTrivia().ToFullString().Contains("A CLong value."),
            "Change Windows field storage without rewriting documentation");
        Require(generatedLayout.GetLeadingTrivia().ToFullString().Contains("<summary>") &&
            generatedLayout.Members.OfType<MethodDeclarationSyntax>().All(m => m.GetLeadingTrivia().ToFullString().Contains("<summary>")),
            "Document generated Windows layout conversions");
        Console.WriteLine("PASS: 19 binding-generator regression checks");
    }

    /// <summary>
    /// Requires unsupported source declarations to fail generation with an explicit diagnostic.
    /// </summary>
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

    /// <summary>
    /// Stops the regression check when a required generated-code invariant is not satisfied.
    /// </summary>
    private static void Require(bool condition, string description)
    {
        if (!condition)
        {
            throw new InvalidOperationException(description);
        }
    }
}
