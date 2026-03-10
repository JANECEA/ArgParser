using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during ArgParser construction. It checks the given type derived from <cref="BaseArgs"/>.
/// </summary>
public class OptionsException : TypeValidationException
{
    internal OptionsException(string? message) : base(message)
    {
    }
}

/// <summary>
/// 
/// </summary>
public class IncorrectNameFormatException : OptionsException
{
    internal IncorrectNameFormatException(string? message) : base(message)
    {
    }
}

/// <summary>
/// 
/// </summary>
public class DuplicateShortOptionException : OptionsException
{
    internal DuplicateShortOptionException(string? message) : base(message)
    {
    }
}

/// <summary>
/// 
/// </summary>
public class DuplicateLongOptionException : OptionsException
{
    internal DuplicateLongOptionException(string? message) : base(message)
    {
    }
}


/// <summary>
/// 
/// </summary>
public class EmptyOptionNameException : OptionsException
{
    internal EmptyOptionNameException(string? message) : base(message)
    {
    }
}

