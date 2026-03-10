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
///
/// </summary>
public sealed class IncorrectNameFormatException : OptionNameException
{
    internal IncorrectNameFormatException(string? message)
        : base(message) { }
}

/// <summary>
///
/// </summary>
public sealed class DuplicateShortOptionException : OptionNameException
{
    internal DuplicateShortOptionException(string? message)
        : base(message) { }
}

/// <summary>
///
/// </summary>
public sealed class DuplicateLongOptionException : OptionNameException
{
    internal DuplicateLongOptionException(string? message)
        : base(message) { }
}

/// <summary>
///
/// </summary>
public sealed class EmptyOptionNameException : OptionNameException
{
    internal EmptyOptionNameException(string? message)
        : base(message) { }
}
