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
///     [ShortOptions('o')]
///     public string Output { get; set; }
/// //
///     [ShortOptions('a')]
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

/// <summary>
/// Declares the boolean property as terminating - throws TException when specified
/// </summary>
/// <typeparam name="TException">Type of the exception, needs to have a public parameterless constructor</typeparam>
[AttributeUsage(AttributeTargets.Property)]
public sealed class TerminatingFlagAttribute<TException> : Attribute
    where TException : Exception, new()
{
    internal void ThrowException() => throw new TException();

    internal TException GetException() => new();
}

/// <summary>
/// Defines how the case of command line option values will be
/// interpreted in relation to the enum constants definition
/// </summary>
public enum EnumCase
{
    /// <summary>
    /// Default value.
    /// This command line option will be expected to preserve case.
    /// </summary>
    /// <example>
    /// Only '--option First' will be interpreted as `First` in
    /// <code>
    /// [EnumCasePolicy(EnumCase.PreserveCase)]
    /// enum MyEnum
    /// {
    ///     First,
    ///     Second,
    /// }
    /// </code>
    /// </example>
    PreserveCase,

    /// <summary>
    /// This command line option will be expected to be lowercase.
    /// </summary>
    /// <example>
    /// Only '--option first' will be interpreted as `First` in
    /// <code>
    /// [EnumCasePolicy(EnumCase.AllLowerCase)]
    /// enum MyEnum
    /// {
    ///     First,
    ///     Second,
    /// }
    /// </code>
    /// </example>
    AllLowerCase,

    /// <summary>
    /// This command line option will be expected to be uppercase
    /// </summary>
    /// <example>
    /// Only '--option FIRST' will be interpreted as `First` in enum:
    /// <code>
    /// [EnumCasePolicy(EnumCase.AllUpperCase)]
    /// enum MyEnum
    /// {
    ///     First,
    ///     Second,
    /// }
    /// </code>
    /// </example>
    AllUpperCase,
}

/// <summary>
/// Specifies how the case of command line option values will be
/// interpreted in relation to the enum constants definition
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumCasePolicy : Attribute
{
    internal EnumCase EnumCase { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="EnumCasePolicy"/>.
    /// </summary>
    public EnumCasePolicy(EnumCase enumCase) => EnumCase = enumCase;
}
