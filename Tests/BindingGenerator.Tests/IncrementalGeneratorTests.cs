using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Exercises compiler-hosted generation, original-source diagnostics, and incremental invalidation.
/// </summary>
[TestClass]
public sealed class IncrementalGeneratorTests
{
    /// <summary>Provides a representative import whose body must come from the generator.</summary>
    private const string Import = "class H5 { [DllImport(Constants.DLLFileName, EntryPoint = \"example\", CallingConvention = CallingConvention.Cdecl)] public static extern int Call(); }";

    /// <summary>Uses an otherwise empty compilation to isolate additional-file transformations.</summary>
    private static readonly CSharpCompilation Compilation = CSharpCompilation.Create("GeneratorTests");

    /// <summary>Checks that generated sources are added without compiler-host exceptions.</summary>
    [TestMethod]
    public void GeneratesBindingsThroughCompilerPipeline()
    {
        var result = Create(new Input("/repo/HDF5/H5.cs", Import)).RunGenerators(Compilation).GetRunResult();
        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
        StringAssert.Contains(result.GeneratedTrees[0].ToString(), "__Legacy0Cache.Function()");
        Assert.IsNull(result.Results[0].Exception);
    }

    /// <summary>Checks that errors point to original additional-file coordinates, including UTF-8 failures.</summary>
    /// <param name="source">The invalid binding declaration.</param>
    /// <param name="line">The expected zero-based source line.</param>
    [TestMethod]
    [DataRow("class H5 {\n[DllImport(\"other\", EntryPoint = \"x\", CallingConvention = CallingConvention.Cdecl)] public static extern int Call();\n}", 1)]
    [DataRow("class H5 {\npublic delegate string Callback();\n}", 1)]
    [DataRow("class H5 {\n[DllImport(Constants.DLLFileName, EntryPoint = \"x\", CallingConvention = CallingConvention.Cdecl)] public static extern string Call();\n}", 1)]
    [DataRow("class H5 {\nstruct Unknown {\n[MarshalAs(UnmanagedType.LPUTF8Str)] public string Text;\n}\n}", 2)]
    public void ReportsOriginalSourceLocations(string source, int line)
    {
        var result = Create(new Input("/repo/HDF5/Bad.cs", source)).RunGenerators(Compilation).GetRunResult();
        var diagnostic = result.Diagnostics.Single();
        Assert.AreEqual("HDFGEN001", diagnostic.Id);
        Assert.AreEqual("/repo/HDF5/Bad.cs", diagnostic.Location.GetLineSpan().Path);
        Assert.AreEqual(line, diagnostic.Location.GetLineSpan().StartLinePosition.Line);
        Assert.IsNull(result.Results[0].Exception);
    }

    /// <summary>Rejects malformed syntax with a source diagnostic rather than a generator crash.</summary>
    [TestMethod]
    public void ReportsSyntaxErrors()
    {
        var result = Create(new Input("/repo/HDF5/Bad.cs", "class {")).RunGenerators(Compilation).GetRunResult();
        Assert.AreEqual("HDFGEN001", result.Diagnostics.Single().Id);
        Assert.IsNull(result.Results[0].Exception);
    }

    /// <summary>Reports malformed import metadata without an unhandled generator exception.</summary>
    /// <param name="attributes">The invalid import attributes to diagnose.</param>
    [TestMethod]
    [DataRow("[DllImport]")]
    [DataRow("[DllImport(Constants.DLLFileName, 1)]")]
    [DataRow("[DllImport(Constants.DLLFileName, EntryPoint = \"a\", EntryPoint = \"b\")]")]
    [DataRow("[DllImport(Constants.DLLFileName)][DllImport(Constants.DLLFileName)]")]
    public void ReportsMalformedImportMetadata(string attributes)
    {
        var result = Create(new Input("/repo/HDF5/Bad.cs", "class H5 { " + attributes + " public static extern int Call(); }"))
            .RunGenerators(Compilation).GetRunResult();
        Assert.AreEqual("HDFGEN001", result.Diagnostics.Single().Id);
        Assert.IsNull(result.Results[0].Exception);
    }

    /// <summary>Rejects method shapes that cannot represent a native import without discarding source semantics.</summary>
    /// <param name="declaration">The invalid method shape following the import attribute.</param>
    [TestMethod]
    [DataRow("public extern int Call();")]
    [DataRow("public static int Call() => 42;")]
    [DataRow("public static extern int Call<T>();")]
    public void ReportsUnsupportedMethodShapes(string declaration)
    {
        string source = Import.Replace("public static extern int Call();", declaration);
        var result = Create(new Input("/repo/HDF5/Bad.cs", source)).RunGenerators(Compilation).GetRunResult();
        Assert.AreEqual("HDFGEN001", result.Diagnostics.Single().Id);
        Assert.IsNull(result.Results[0].Exception);
    }
    /// <summary>Checks that unreadable files and missing imports cannot silently produce an empty assembly.</summary>
    [TestMethod]
    public void ReportsMissingOrUnreadableInputs()
    {
        Assert.AreEqual("HDFGEN003", Create().RunGenerators(Compilation).GetRunResult().Diagnostics.Single().Id);
        Assert.AreEqual("HDFGEN003", Create(new Input("/repo/HDF5/Empty.cs", "class H5 {}"))
            .RunGenerators(Compilation).GetRunResult().Diagnostics.Single().Id);
        Assert.AreEqual("HDFGEN002", Create(new Input("/repo/HDF5/Missing.cs", null))
            .RunGenerators(Compilation).GetRunResult().Diagnostics.Single().Id);
    }

    /// <summary>Uses actual compiler symbols and invalidates cached output when those symbols change.</summary>
    [TestMethod]
    public void HonorsChangedPreprocessorSymbols()
    {
        var file = new Input("/repo/HDF5/H5.cs", "#if CUSTOM\n" + Import + "\n#endif");
        var driver = Create(file).RunGenerators(Compilation);
        Assert.AreEqual("HDFGEN003", driver.GetRunResult().Diagnostics.Single().Id);
        driver = driver.WithUpdatedParseOptions(new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: new[] { "CUSTOM" })).RunGenerators(Compilation);
        Assert.AreEqual(0, driver.GetRunResult().Diagnostics.Length);
        StringAssert.Contains(driver.GetRunResult().GeneratedTrees.Single().ToString(), "__Legacy0Cache");
    }

    /// <summary>Does not regenerate unchanged files when the compilation or another source changes.</summary>
    [TestMethod]
    public void ReusesUnchangedTransformations()
    {
        var first = new Input("/repo/HDF5/First.cs", Import);
        var second = new Input("/repo/HDF5/Second.cs", Import.Replace("class H5", "class H5F"));
        var driver = Create(first, second).RunGenerators(Compilation);
        driver = driver.RunGenerators(Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText("class Unrelated {}")));
        Assert.IsTrue(Reasons(driver).All(reason => reason == IncrementalStepRunReason.Cached));

        driver = driver.ReplaceAdditionalText(first, new Input(first.Path, Import.Replace("example", "changed"))).RunGenerators(Compilation);
        CollectionAssert.AreEquivalent(new[] { IncrementalStepRunReason.Modified, IncrementalStepRunReason.Cached }, Reasons(driver));
    }

    /// <summary>Removes obsolete generated trees and supports equal basenames in different source directories.</summary>
    [TestMethod]
    public void HandlesRemovalAndDistinctRelativePaths()
    {
        var first = new Input("/repo/HDF5/One/H5.cs", Import);
        var second = new Input("/repo/HDF5/Two/H5.cs", Import.Replace("class H5", "class H5F"));
        var driver = Create(first, second).RunGenerators(Compilation);
        Assert.AreEqual(2, driver.GetRunResult().Results[0].GeneratedSources.Select(source => source.HintName).Distinct().Count());
        driver = driver.RemoveAdditionalTexts(ImmutableArray.Create<AdditionalText>(first)).RunGenerators(Compilation);
        Assert.AreEqual(1, driver.GetRunResult().GeneratedTrees.Length);
        StringAssert.Contains(driver.GetRunResult().GeneratedTrees.Single().ToString(), "class H5F");
    }

    /// <summary>Produces identical source names and content when a checkout moves to another directory.</summary>
    [TestMethod]
    public void GeneratesDeterministicOutputAcrossCheckouts()
    {
        var first = Create(new Input("/one/HDF5/H5.cs", Import)).RunGenerators(Compilation).GetRunResult().Results[0].GeneratedSources.Single();
        var second = Create(new Input("/two/HDF5/H5.cs", Import)).RunGenerators(Compilation).GetRunResult().Results[0].GeneratedSources.Single();
        Assert.AreEqual(first.HintName, second.HintName);
        Assert.AreEqual(first.SourceText.ToString(), second.SourceText.ToString());
    }

    /// <summary>Ignores unrelated additional files without interpreting them as binding declarations.</summary>
    [TestMethod]
    public void IgnoresUnmarkedAdditionalFiles()
    {
        var result = Create(new Input("/repo/HDF5/H5.cs", Import), new Input("/repo/other.txt", "not C#"))
            .RunGenerators(Compilation).GetRunResult();
        Assert.AreEqual(0, result.Diagnostics.Length);
        Assert.AreEqual(1, result.GeneratedTrees.Length);
    }

    /// <summary>Allows compiler cancellation to propagate without reporting it as a binding error.</summary>
    [TestMethod]
    public void HonorsCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Assert.ThrowsExactly<OperationCanceledException>(() => BindingSource.Generate(Import, out _, cancellationToken: cancellation.Token));
    }

    /// <summary>Creates a tracked generator driver with explicit additional-file selection metadata.</summary>
    private static GeneratorDriver Create(params Input[] files) => CSharpGeneratorDriver.Create(
        ImmutableArray.Create(new LegacyBindingGenerator().AsSourceGenerator()), files.Cast<AdditionalText>().ToImmutableArray(),
        new CSharpParseOptions(LanguageVersion.Latest), new OptionsProvider(), new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

    /// <summary>Extracts per-file transformation reasons from the generator's tracked pipeline.</summary>
    private static IncrementalStepRunReason[] Reasons(GeneratorDriver driver) => driver.GetRunResult().Results[0].TrackedSteps["LegacyBindings"]
        .SelectMany(step => step.Outputs).Select(output => output.Reason).ToArray();

    /// <summary>Provides an immutable in-memory additional file without touching the filesystem.</summary>
    private sealed class Input(string path, string? text) : AdditionalText
    {
        /// <summary>Gets the source path used for diagnostics and input identity.</summary>
        public override string Path => path;

        /// <summary>Returns the source text, or null to simulate an unreadable file.</summary>
        public override SourceText? GetText(CancellationToken cancellationToken = default) => text == null ? null : SourceText.From(text);
    }

    /// <summary>Supplies the same input-selection metadata as the library project's AdditionalFiles items.</summary>
    private sealed class OptionsProvider : AnalyzerConfigOptionsProvider
    {
        /// <summary>Provides empty global options because this generator does not read global settings.</summary>
        public override AnalyzerConfigOptions GlobalOptions => new Options(null);

        /// <summary>Provides empty options for ordinary compilation trees.</summary>
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new Options(null);

        /// <summary>Selects only fixture paths beneath the shared HDF5 source directory.</summary>
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            int start = textFile.Path.IndexOf("/HDF5/", StringComparison.Ordinal);
            return new Options(start < 0 ? null : textFile.Path.Substring(start + 1));
        }
    }

    /// <summary>Exposes the portable relative path used to identify one binding input.</summary>
    private sealed class Options(string? relativePath) : AnalyzerConfigOptions
    {
        /// <summary>Retrieves the binding-selection metadata when it exists.</summary>
        public override bool TryGetValue(string key, out string value)
        {
            value = relativePath ?? string.Empty;
            return key == "build_metadata.AdditionalFiles.HdfBindingPath" && relativePath != null;
        }
    }
}
