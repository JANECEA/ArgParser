using ArgParser.Analyzers.Abstractions;

namespace ArgParser.Attributes;

/// <summary>
/// Overrides the option value placeholder name used in the --help message
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ValuePlaceholderAttribute : Attribute, IOnParsable
{
    internal string PlaceHolder { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ValuePlaceholderAttribute"/>.
    /// </summary>
    public ValuePlaceholderAttribute(string placeHolder)
    {
        PlaceHolder = placeHolder;
    }
}

/// <summary>
/// Specifies the example usage string used in the --help message
/// </summary>
/// <example>
/// <code>
/// [ExampleUsage("time [options] command [arguments...]")]
/// internal sealed class TimeArgs : BaseArgs
/// {
///     [
///         ShortOptions('a'),
///         Help("(Used together with -o.) Do not overwrite but append."),
///     ]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExampleUsageAttribute : Attribute, IOnClassType<BaseArgs>
{
    internal string Usage { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ExampleUsageAttribute"/>.
    /// </summary>
    public ExampleUsageAttribute(string usage)
    {
        Usage = usage;
    }
}

/// <summary>
/// Defines meaning of this command line option.
/// Used when '--help' is called.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class HelpAttribute : Attribute, IOnParsable
{
    internal string Description { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="HelpAttribute"/>.
    /// </summary>
    /// <param name="description">Help description</param>
    public HelpAttribute(string description)
    {
        Description = description;
    }
}
