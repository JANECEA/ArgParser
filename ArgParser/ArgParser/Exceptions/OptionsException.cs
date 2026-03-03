using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

public class OptionsException : TypeValidationException
{
    internal OptionsException(string? message) : base(message)
    {
    }
}

public class IncorrectNameFormatException : OptionsException
{
    internal IncorrectNameFormatException(string? message) : base(message)
    {
    }
}


public class DuplicateShortOptionException : OptionsException
{
    internal DuplicateShortOptionException(string? message) : base(message)
    {
    }
}

public class DuplicateLongOptionException : OptionsException
{
    internal DuplicateLongOptionException(string? message) : base(message)
    {
    }
}


public class EmptyOptionNameException : OptionsException
{
    internal EmptyOptionNameException(string? message) : base(message)
    {
    }
}

