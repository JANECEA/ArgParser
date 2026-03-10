using ArgParser.Attributes;

namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during ArgParser construction. It checks the given type derived from <see cref="BaseArgs"/>.
/// </summary>
public abstract class OptionNameException : ParserConfigurationException
{
    internal OptionNameException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when <see cref="ShortOptionsAttribute"/> or <see cref="LongOptionsAttribute"/> have incorrect format.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [LongOptions("append option")]
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
/// The exception that is thrown when multiple <see cref="ShortOptionsAttribute"/>s have the same value.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions('a')]
///     public bool Append { get; set; }
///
/// //
///     [ShortOptions('a')]
///     public bool Allow { get; set; }
/// }
/// </code>
/// </example>
public sealed class DuplicateShortOptionException : OptionNameException
{
    internal DuplicateShortOptionException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when multiple <see cref="LongOptionsAttribute"/>s have the same value.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [LongOptions("App")]
///     public bool Append { get; set; }
///
/// //
///     [LongOptions("App")]
///     public bool Allow { get; set; }
/// }
/// </code>
/// </example>
public sealed class DuplicateLongOptionException : OptionNameException
{
    internal DuplicateLongOptionException(string? message)
        : base(message) { }
}
