using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

/// <summary>
/// Generates the legacy bindings from explicitly selected additional C# files without rewriting build outputs.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class LegacyBindingGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Describes source declarations whose ABI or ownership rules cannot be generated safely.
    /// </summary>
    private static readonly DiagnosticDescriptor InvalidSource = new DiagnosticDescriptor(
        "HDFGEN001", "Cannot generate legacy binding", "{0}", "HDF.PInvoke", DiagnosticSeverity.Error, true);

    /// <summary>
    /// Describes additional files that the compiler host could not read.
    /// </summary>
    private static readonly DiagnosticDescriptor UnreadableSource = new DiagnosticDescriptor(
        "HDFGEN002", "Cannot read legacy binding source", "Cannot read binding source '{0}'", "HDF.PInvoke", DiagnosticSeverity.Error, true);

    /// <summary>
    /// Prevents a missing input selection from silently producing an incomplete legacy assembly.
    /// </summary>
    private static readonly DiagnosticDescriptor MissingImports = new DiagnosticDescriptor(
        "HDFGEN003", "No legacy imports found", "No HDF5 imports were found in the selected binding sources", "HDF.PInvoke", DiagnosticSeverity.Error, true);

    /// <summary>
    /// Registers per-file transformations, diagnostics, and an aggregate check for missing native imports.
    /// </summary>
    /// <param name="context">The compiler's incremental generation context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var inputs = context.AdditionalTextsProvider.Combine(context.AnalyzerConfigOptionsProvider)
            .Select((input, cancellation) =>
            {
                input.Right.GetOptions(input.Left).TryGetValue("build_metadata.AdditionalFiles.HdfBindingPath", out var relativePath);
                return (File: input.Left, RelativePath: relativePath);
            })
            .Where(input => !string.IsNullOrEmpty(input.RelativePath))
            .Select((input, cancellation) => (input.File.Path, RelativePath: input.RelativePath!.Replace('\\', '/'),
                Text: input.File.GetText(cancellation)?.ToString()));

        // Value-equatable inputs prevent unrelated compilation changes from regenerating every binding.
        var bindings = inputs.Combine(context.ParseOptionsProvider)
            .Select((input, cancellation) => Generate(input.Left.Path, input.Left.RelativePath, input.Left.Text,
                (CSharpParseOptions)input.Right, cancellation))
            .WithTrackingName("LegacyBindings");

        context.RegisterSourceOutput(bindings, (output, binding) =>
        {
            if (binding.Diagnostic != null)
            {
                output.ReportDiagnostic(binding.Diagnostic);
            }
            else
            {
                output.AddSource(binding.HintName, SourceText.From(binding.Source!, Encoding.UTF8));
            }
        });

        // Only small count/error values are collected; source generation remains independent per file.
        var counts = bindings.Select((binding, cancellation) => (binding.ImportCount, Failed: binding.Diagnostic != null)).Collect();
        context.RegisterSourceOutput(counts, (output, values) =>
        {
            if (!values.Any(value => value.Failed) && values.Sum(value => value.ImportCount) == 0)
            {
                output.ReportDiagnostic(Diagnostic.Create(MissingImports, Location.None));
            }
        });
    }

    /// <summary>
    /// Transforms one input, retaining failures as compiler diagnostics instead of throwing out the generator.
    /// </summary>
    private static GeneratedBinding Generate(string path, string relativePath, string? text, CSharpParseOptions options, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();
        var start = Location.Create(path, new TextSpan(0, 0), new LinePositionSpan(new LinePosition(0, 0), new LinePosition(0, 0)));

        if (text == null)
        {
            return new GeneratedBinding(Diagnostic.Create(UnreadableSource, start, path));
        }

        try
        {
            var source = BindingSource.Generate(text, out var count, parseOptions: options, cancellationToken: cancellation, path: path);
            return new GeneratedBinding(HintName(relativePath), source, count);
        }
        catch (BindingGenerationException error)
        {
            // Additional files are not compilation syntax trees, so diagnostics use external-file locations.
            var location = error.SourceLocation;
            var span = location.GetLineSpan();
            var external = span.Path == path ? Location.Create(path, location.SourceSpan, span.Span) : start;
            return new GeneratedBinding(Diagnostic.Create(InvalidSource, external, error.Message));
        }
    }

    /// <summary>
    /// Produces a stable, collision-resistant hint name independent of the checkout's absolute location.
    /// </summary>
    private static string HintName(string relativePath)
    {
        using (var hash = SHA256.Create())
        {
            var digest = BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(relativePath))).Replace("-", "");
            return Path.GetFileName(relativePath) + "." + digest + ".g.cs";
        }
    }

    /// <summary>
    /// Holds either one generated source or its diagnostic and the native-import count used for validation.
    /// </summary>
    private sealed class GeneratedBinding
    {
        /// <summary>
        /// Creates a successful per-file transformation result.
        /// </summary>
        internal GeneratedBinding(string hintName, string source, int importCount)
        {
            HintName = hintName;
            Source = source;
            ImportCount = importCount;
        }

        /// <summary>
        /// Creates a failed per-file transformation result.
        /// </summary>
        internal GeneratedBinding(Diagnostic diagnostic)
        {
            Diagnostic = diagnostic;
            HintName = string.Empty;
        }

        /// <summary>Gets the stable generated-file identity.</summary>
        internal string HintName
        {
            get;
        }

        /// <summary>Gets the generated C# source when transformation succeeded.</summary>
        internal string? Source
        {
            get;
        }

        /// <summary>Gets the number of transformed native imports.</summary>
        internal int ImportCount
        {
            get;
        }

        /// <summary>Gets the source diagnostic when transformation failed.</summary>
        internal Diagnostic? Diagnostic
        {
            get;
        }
    }
}
