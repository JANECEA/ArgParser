using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

public class TypeValidationException : InvalidConstraintException
{
    public TypeValidationException(string? message) : base(message) { }
}



public class PropertyNotParsableException : TypeValidationException
{
    public PropertyNotParsableException(string? message) : base(message)
    {
    }
}


public class RequiresPropertyNotFoundException : TypeValidationException
{
    public RequiresPropertyNotFoundException(string? message) : base(message)
    {
    }
}


public class WrongAttributeTypeException : TypeValidationException
{
    public WrongAttributeTypeException(string? message) : base(message)
    {
    }
}

public class RequiredOnFlagException : TypeValidationException
{
    public RequiredOnFlagException(string? message) : base(message)
    {
    }
}

