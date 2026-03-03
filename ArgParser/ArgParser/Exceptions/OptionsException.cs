using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

public class OptionsException : TypeValidationException
{
    public OptionsException(string? message) : base(message)
    {
    }
}


public class DuplicateShortOptionException : OptionsException
{
    public DuplicateShortOptionException(string? message) : base(message)
    {
    }
}

public class DuplicateLongOptionException : OptionsException
{
    public DuplicateLongOptionException(string? message) : base(message)
    {
    }
}


public class EmptyOptionNameException : OptionsException
{
    public EmptyOptionNameException(string? message) : base(message)
    {
    }
}

