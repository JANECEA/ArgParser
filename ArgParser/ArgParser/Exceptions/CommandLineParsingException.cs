namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown while parsing command line arguments.
/// </summary>
public abstract class CommandLineParsingException : Exception
{
    internal CommandLineParsingException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when required command line option is missing.
/// </summary>
public sealed class MissingRequiredOptionException : CommandLineParsingException
{
    internal MissingRequiredOptionException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the value specified for command line option did not pass defined validation.
/// </summary>
public sealed class ValidatorFailedException : CommandLineParsingException
{
    internal ValidatorFailedException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the value for command line option is missing.
/// </summary>
public sealed class MissingOptionValueException : CommandLineParsingException
{
    internal MissingOptionValueException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the value for command line option could not be parsed.
/// </summary>
public sealed class ValueParsingException : CommandLineParsingException
{
    internal ValueParsingException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when unknown command line option was given.
/// </summary>
public sealed class UnknownOptionException : CommandLineParsingException
{
    internal UnknownOptionException(string? message)
        : base(message) { }
}


