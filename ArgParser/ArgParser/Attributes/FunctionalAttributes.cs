namespace ArgParser.Attributes;

/// <summary>
/// Specifies if the command line option is required.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredAttribute : Attribute;

/// <summary>
/// Specifies the short form of a command-line option.
/// <para/>
/// The single dash (<c>-</c>) prefix will be prepended automatically.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions('o')]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ShortOptionsAttribute : Attribute
{
    internal IEnumerable<char> Options { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ShortOptionsAttribute"/>.
    /// </summary>
    public ShortOptionsAttribute(char mainOptionName, params char[] otherOptions)
    {
        Options = otherOptions.Prepend(mainOptionName);
    }
}

/// <summary>
/// Specifies the long form of a command-line option.
/// <para/>
/// The double dash (<c>--</c>) prefix will be prepended automatically.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [LongOptions("output")]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LongOptionsAttribute : Attribute
{
    internal IEnumerable<string> Options { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="LongOptionsAttribute"/>.
    /// </summary>
    public LongOptionsAttribute(string mainOptionName, params string[] otherOptions)
    {
        Options = otherOptions.Prepend(mainOptionName);
    }
}

/// <summary>
/// Allows the user to define dependencies on another options.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions("-o")]
///     public string Output { get; set; }
/// //
///     [ShortOptions("-a")]
///     [Requires(nameof(Output))]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiresAttribute : Attribute
{
    internal string[] PropertyName { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="RequiredAttribute"/>.
    /// </summary>
    /// <param name="propertyName">Required property names</param>
    public RequiresAttribute(params string[] propertyName)
    {
        PropertyName = propertyName;
    }
}
