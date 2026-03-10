namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during parsing command line arguments.
/// </summary>
public abstract class CommandLineParsingException : Exception
{
    internal CommandLineParsingException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when required command line option is missing.
/// </summary>
public class MissingRequiredOptionException : CommandLineParsingException
{
    internal MissingRequiredOptionException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the value specified for command line option did not pass defined validation.
/// </summary>
public class ValidatorFailedException : CommandLineParsingException
{
    internal ValidatorFailedException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the value for command line option is missing.
/// </summary>
public class MissingOptionValueException : CommandLineParsingException
{
    internal MissingOptionValueException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the value for command line option has incorrect format.
/// </summary>
public class ValueFormatException : CommandLineParsingException
{
    internal ValueFormatException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when unknown command line option was given.
/// </summary>
public class UnknownOptionException : CommandLineParsingException
{
    internal UnknownOptionException(string? message)
        : base(message) { }
}
