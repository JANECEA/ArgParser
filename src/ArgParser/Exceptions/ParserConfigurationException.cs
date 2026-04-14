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
public sealed class PropertyNotParsableException : ParserConfigurationException
{
    internal PropertyNotParsableException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the option property given to the <see cref="RequiresAttribute"/>
/// was not found in the given class.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortNames('a')]
///     [Requires(nameof(Output))]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
public sealed class ReferencedOptionNotFoundException : ParserConfigurationException
{
    internal ReferencedOptionNotFoundException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the <see cref="ClassValidatorAttribute{TArgs}"/> or the <see cref="OptionValidatorAttribute{TType}"/>
/// have different type than the property.
/// </summary>
/// <example>
/// <code>
///
/// class Args : BaseArgs
/// {
///     [ShortNames('a')]
///     [Range{int}(0, 100)]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
public sealed class WrongValidatorTypeException : ParserConfigurationException
{
    internal WrongValidatorTypeException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the <see cref="RequiredAttribute"/> is registered on flag property in the given class.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortNames('a')]
///     [Required]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
public sealed class RequiredOnFlagException : ParserConfigurationException
{
    internal RequiredOnFlagException(string? message)
        : base(message) { }
}

/// <summary>
/// The exception that is thrown when the <see cref="TerminatingFlagAttribute{TException}"/>
/// is registered on a property that is not of type <see cref="bool"/>.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortNames('o')]
///     [TerminatingFlag{Exception}]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
public sealed class TerminatingNotOnFlagException : ParserConfigurationException
{
    internal TerminatingNotOnFlagException(string? message)
        : base(message) { }
}
