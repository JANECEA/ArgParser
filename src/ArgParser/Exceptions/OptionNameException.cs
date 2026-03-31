using ArgParser.Attributes;

namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions related to incorrect name format
/// </summary>
public abstract class OptionNameException : ParserConfigurationException
{
    internal OptionNameException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when <see cref="ShortNamesAttribute"/> or <see cref="LongNamesAttribute"/> have incorrect format.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [LongNames("append option")]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
public sealed class IncorrectNameFormatException : OptionNameException
{
    internal IncorrectNameFormatException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when multiple LongNames or ShortNames have the same value.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortNames('a')]
///     public bool Append { get; set; }
///
/// //
///     [ShortNames('a')]
///     public bool Allow { get; set; }
/// }
/// </code>
/// </example>
public sealed class DuplicateOptionNameException : OptionNameException
{
    internal DuplicateOptionNameException(string? message)
        : base(message) { }
}
