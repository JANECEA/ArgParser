using ArgParser.Analyzers.Abstractions;
using ArgParser.Exceptions;
using ArgParser.Internal;

namespace ArgParser.Attributes;

/// <summary>
/// Specifies if the command line option is required.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredAttribute : Attribute, INotOnFlag, IOnParsable;

/// <summary>
/// Specifies the short form of a command-line option.
/// <para/>
/// The single dash (<c>-</c>) prefix will be prepended automatically.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortNames('o')]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ShortNamesAttribute : Attribute, IOnParsable
{
    internal List<char> Names { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ShortNamesAttribute"/>.
    /// </summary>
    public ShortNamesAttribute(char mainOptionName, params char[] otherOptions)
    {
        List<char> list = otherOptions.Prepend(mainOptionName).ToList();

        foreach (char c in list)
        {
            if (!char.IsAsciiLetter(c))
                throw new IncorrectNameFormatException($"Incorrect short name: {c}");
        }

        Names = list;
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
///     [LongNames("output")]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LongNamesAttribute : Attribute, IOnParsable
{
    private static readonly char[] AllowedChars = ['-', '.', ':', '_'];

    internal List<string> Names { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="LongNamesAttribute"/>.
    /// </summary>
    public LongNamesAttribute(string mainOptionName, params string[] otherOptions)
    {
        List<string> list = otherOptions.Prepend(mainOptionName).ToList();

        foreach (string opt in list)
        {
            if (!ValidateOptionFormat(opt))
                throw new IncorrectNameFormatException($"Incorrect long name: {opt}");
        }

        Names = list;
    }

    private static bool ValidateOptionFormat(string opt)
    {
        if (string.IsNullOrWhiteSpace(opt))
            return false;

        if (char.IsAsciiDigit(opt[0]) || AllowedChars.Contains(opt[0]))
            return false;

        foreach (char c in opt)
            if (!char.IsAsciiLetterOrDigit(c) && !AllowedChars.Contains(c))
                return false;

        return true;
    }
}

/// <summary>
/// Allows the user to define dependencies on another options.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortNames('o')]
///     public string Output { get; set; }
/// //
///     [ShortNames('a')]
///     [Requires(nameof(Output))]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiresAttribute : Attribute, IOnParsable
{
    internal List<string> PropertyNames { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="RequiredAttribute"/>.
    /// </summary>
    public RequiresAttribute(string firstPropertyName, params string[] otherPropertyNames)
    {
        PropertyNames = otherPropertyNames.Prepend(firstPropertyName).ToList();
    }
}

/// <summary>
/// Declares the boolean property as terminating - throws TException when specified
/// </summary>
/// <typeparam name="TException">Type of the exception, needs to have a public parameterless constructor</typeparam>
[AttributeUsage(AttributeTargets.Property)]
public sealed class TerminatingFlagAttribute<TException> : Attribute, ITerminatingFlag, IOnFlag
    where TException : Exception, new()
{
    public void ThrowException() => throw new TException();
}

/// <summary>
/// Defines how the case of command line option values will be
/// interpreted in relation to the enum constants definition.
/// </summary>
public enum EnumCase
{
    /// <summary>
    /// Default value.
    /// This command line option will be expected to preserve case.
    /// </summary>
    /// <example>
    /// Only '-e First' will be interpreted as `First` for enum:
    /// <code>
    /// enum MyEnum { First, Second }
    ///
    /// [ShortNames('e')]
    /// [EnumCasePolicy(EnumCase.PreserveCase)]
    /// public MyEnum enumOption { get; set; }
    /// </code>
    /// </example>
    PreserveCase,

    /// <summary>
    /// This command line option will be expected to be lowercase.
    /// </summary>
    /// <example>
    /// Only '-e first' will be interpreted as `First` for enum:
    /// <code>
    /// enum MyEnum { First, Second }
    ///
    /// [ShortNames('e')]
    /// [EnumCasePolicy(EnumCase.AllLowerCase)]
    /// public MyEnum enumOption { get; set; }
    /// </code>
    /// </example>
    AllLowerCase,

    /// <summary>
    /// This command line option will be expected to be uppercase.
    /// </summary>
    /// <example>
    /// Only '-e FIRST' will be interpreted as `First` for enum:
    /// <code>
    /// enum MyEnum { First, Second }
    ///
    /// [ShortNames('e')]
    /// [EnumCasePolicy(EnumCase.AllUpperCase)]
    /// public MyEnum enumOption { get; set; }
    /// </code>
    /// </example>
    AllUpperCase,
}

/// <summary>
/// Specifies how the case of command line option values will be
/// interpreted in relation to the enum constants definition.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class EnumCasePolicyAttribute : Attribute
{
    internal EnumCase Case { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="EnumCasePolicyAttribute"/>.
    /// </summary>
    public EnumCasePolicyAttribute(EnumCase enumCase) => Case = enumCase;
}

/// <summary>
/// Defines the order positional typed plain arguments.
/// <br/>
/// Cannot be mixed with <see cref="ShortNamesAttribute"/>,
/// <see cref="LongNamesAttribute"/>, or
/// <see cref="TerminatingFlagAttribute{TException}"/>
/// on the referenced properties.
/// <br/>
/// Can be validated using <see cref="OptionValidatorAttribute{TType}"/> and marked
/// with <see cref="RequiredAttribute"/> and <see cref="RequiresAttribute"/>.
/// <example>
/// <code>
/// [PositionalArgs(nameof(IntArg), nameof(StrArg))]
/// class Args : BaseArgs
/// {
///     public int IntArg { get; set; }
///     public string StrArg { get; set; }
/// }
/// </code>
/// </example>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class PositionalArgsAttribute : Attribute
{
    internal List<string> PropertyNames { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="PositionalArgsAttribute"/>.
    /// </summary>
    public PositionalArgsAttribute(string firstPropertyName, params string[] otherPropertyNames)
    {
        PropertyNames = otherPropertyNames.Prepend(firstPropertyName).ToList();
    }
}
