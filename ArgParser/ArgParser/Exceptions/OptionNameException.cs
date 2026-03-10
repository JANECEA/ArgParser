namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during ArgParser construction. It checks the given type derived from <see cref="BaseArgs"/>.
/// </summary>
public class OptionNameException : ParserConfigurationException
{
    internal OptionNameException(string? message)
        : base(message) { }
}

/// <summary>
///
/// </summary>
public class IncorrectNameFormatException : OptionNameException
{
    internal IncorrectNameFormatException(string? message)
        : base(message) { }
}

/// <summary>
///
/// </summary>
public class DuplicateShortOptionException : OptionNameException
{
    internal DuplicateShortOptionException(string? message)
        : base(message) { }
}

/// <summary>
///
/// </summary>
public class DuplicateLongOptionException : OptionNameException
{
    internal DuplicateLongOptionException(string? message)
        : base(message) { }
}

/// <summary>
///
/// </summary>
public class EmptyOptionNameException : OptionNameException
{
    internal EmptyOptionNameException(string? message)
        : base(message) { }
}
