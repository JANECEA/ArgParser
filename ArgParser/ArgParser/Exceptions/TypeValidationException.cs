using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during ArgParser construction. It checks the given type derived from <cref="BaseArgs"/>.
/// </summary>
public class TypeValidationException : InvalidConstraintException
{
    internal TypeValidationException(string? message) : base(message) { }
}

/// <summary>
/// The exception that is thrown when property type does not implement the IParsable interface.
/// </summary>
public class PropertyNotParsableException : TypeValidationException
{
    internal PropertyNotParsableException(string? message) : base(message)
    {
    }
}

/// <summary>
/// The exception that is thrown when the option property given to the <cref="RequiresAttribute"/> was not found in the given class.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions("-a")]
///     [Requires(nameof(Output))]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
public class RequiresOptionNotFoundException : TypeValidationException
{
    internal RequiresOptionNotFoundException(string? message) : base(message)
    {
    }
}

/// <summary>
/// The exception that is thrown when 
/// </summary>
public class WrongAttributeTypeException : TypeValidationException
{
    internal WrongAttributeTypeException(string? message) : base(message)
    {
    }
}

/// <summary>
/// The exception that is thrown when the <cref="RequiredAttribute"/> is registered on flag property in the given class.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions("-a")]
///     [Required]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
public class RequiredOnFlagException : TypeValidationException
{
    internal RequiredOnFlagException(string? message) : base(message)
    {
    }
}

