using Microsoft.CodeAnalysis;

/// <summary>
/// Reports an unsupported declaration or syntax error at its original source location.
/// </summary>
internal sealed class BindingGenerationException : InvalidOperationException
{
    /// <summary>
    /// Captures the diagnostic message and the declaration that prevents generation.
    /// </summary>
    internal BindingGenerationException(string message, Location location) : base(message)
    {
        SourceLocation = location;
    }

    /// <summary>
    /// Gets the original declaration's location, before source transformation changes its offsets.
    /// </summary>
    internal Location SourceLocation
    {
        get;
    }
}
