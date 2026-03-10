using ArgParser.Attributes;

namespace ArgParser.Exceptions;

/// <summary>
/// Base class for exceptions thrown during ArgParser construction. It checks the given type derived from <see cref="BaseArgs"/>.
/// </summary>
public class ParserConfigurationException : Exception
{
    internal ParserConfigurationException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when property type does not implement the IParsable interface.
/// </summary>
public class PropertyNotParsableException : ParserConfigurationException
{
    internal PropertyNotParsableException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the option property given to the <see cref="RequiresAttribute"/> was not found in the given class.
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
public class RequiresOptionNotFoundException : ParserConfigurationException
{
    internal RequiresOptionNotFoundException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when
/// </summary>
public class WrongAttributeTypeException : ParserConfigurationException
{
    internal WrongAttributeTypeException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the <see cref="RequiredAttribute"/> is registered on flag property in the given class.
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
public class RequiredOnFlagException : ParserConfigurationException
{
    internal RequiredOnFlagException(string? message)
        : base(message) { }
}
