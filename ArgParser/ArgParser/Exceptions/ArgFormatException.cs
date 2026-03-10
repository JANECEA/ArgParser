using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during parsing command line arguments.
/// </summary>
public abstract class ArgFormatException : Exception
{
    internal ArgFormatException(string? message) : base(message) { }
}

/// <summary>
/// The exception that is thrown when required command line option is missing.
/// </summary>
public class MissingRequiredOptionException : ArgFormatException
{
    internal MissingRequiredOptionException(string? message) : base(message)
    {
    }
}

/// <summary>
/// The exception that is thrown when the value specified for command line option did not pass defined validation.
/// </summary>
public class ValidatorFailedException : ArgFormatException
{
    internal ValidatorFailedException(string? message) : base(message)
    {
    }
}

/// <summary>
/// The exception that is thrown when the value for command line option is missing.
/// </summary>
public class MissingOptionValueException : ArgFormatException
{
    internal MissingOptionValueException(string? message) : base(message)
    {
    }
}

/// <summary>
/// The exception that is thrown when the value for command line option has incorrect format.
/// </summary>
public class ValueFormatException : ArgFormatException
{
    internal ValueFormatException(string? message) : base(message)
    {
    }
}

/// <summary>
/// The exception that is thrown when unknown command line option was given.
/// </summary>
public class UnknownOptionException : ArgFormatException
{
    internal UnknownOptionException(string? message) : base(message)
    {
    }
}


