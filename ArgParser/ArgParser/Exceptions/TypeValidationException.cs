using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during ArgParser construction. It checks the type 
/// </summary>
public class TypeValidationException : InvalidConstraintException
{
    internal TypeValidationException(string? message) : base(message) { }
}


public class PropertyNotParsableException : TypeValidationException
{
    internal PropertyNotParsableException(string? message) : base(message)
    {
    }
}


public class RequiresOptionNotFoundException : TypeValidationException
{
    internal RequiresOptionNotFoundException(string? message) : base(message)
    {
    }
}


public class WrongAttributeTypeException : TypeValidationException
{
    internal WrongAttributeTypeException(string? message) : base(message)
    {
    }
}

public class RequiredOnFlagException : TypeValidationException
{
    internal RequiredOnFlagException(string? message) : base(message)
    {
    }
}

