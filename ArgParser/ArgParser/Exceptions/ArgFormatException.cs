using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

public class ArgFormatException : Exception
{
    public ArgFormatException(string? message) : base(message) { }
}


public class MissingRequiredOptionException : ArgFormatException
{
    public MissingRequiredOptionException(string? message) : base(message)
    {
    }
}

public class ValidatorFailedException : ArgFormatException
{
    public ValidatorFailedException(string? message) : base(message)
    {
    }
}

public class MissingOptionValueException : ArgFormatException
{
    public MissingOptionValueException(string? message) : base(message)
    {
    }
}

public class ValueFormatException : ArgFormatException
{
    public ValueFormatException(string? message) : base(message)
    {
    }
}

public class UnknownOptionException : ArgFormatException
{
    public UnknownOptionException(string? message) : base(message)
    {
    }
}


