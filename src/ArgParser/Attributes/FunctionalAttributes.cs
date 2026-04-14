using System.Data;
using ArgParser.Analyzers.Abstractions;
using ArgParser.Exceptions;

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
    internal List<char> Options { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ShortNamesAttribute"/>.
    /// </summary>
    public ShortNamesAttribute(char mainOptionName, params char[] otherOptions)
    {
        List<char> list = otherOptions.Prepend(mainOptionName).ToList();
        HashSet<char> set = new();

        foreach (char c in list)
        {
            if (!set.Add(c))
                throw new DuplicateNameException($"Duplicate short name: {c}");

            if (!char.IsAsciiLetter(c))
                throw new IncorrectNameFormatException($"Incorrect short name: {c}");
        }

        Options = list;
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

    internal List<string> Options { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="LongNamesAttribute"/>.
    /// </summary>
    public LongNamesAttribute(string mainOptionName, params string[] otherOptions)
    {
        List<string> list = otherOptions.Prepend(mainOptionName).ToList();
        HashSet<string> set = new();

        foreach (string opt in list)
        {
            if (!set.Add(opt))
                throw new DuplicateNameException($"Duplicate short name: {opt}");

            if (!ValidateOptionFormat(opt))
                throw new IncorrectNameFormatException($"Incorrect short name: {opt}");
        }

        Options = list;
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
public sealed class TerminatingFlagAttribute<TException> : Attribute, IOnFlag
    where TException : Exception, new()
{
    internal void ThrowException() => throw new TException();

    internal TException GetException() => new();
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
    /// Only '--option First' will be interpreted as `First` for enum:
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
    /// Only '--option first' will be interpreted as `First` for enum:
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
    /// This command line option will be expected to be uppercase.
    /// </summary>
    /// <example>
    /// Only '--option FIRST' will be interpreted as `First` for enum:
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
/// interpreted in relation to the enum constants definition.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumCasePolicyAttribute : Attribute
{
    internal EnumCase Case { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="EnumCasePolicyAttribute"/>.
    /// </summary>
    public EnumCasePolicyAttribute(EnumCase enumCase) => Case = enumCase;
}
