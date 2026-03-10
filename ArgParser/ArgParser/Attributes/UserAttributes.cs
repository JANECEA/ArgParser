namespace ArgParser.Attributes;

/// <summary>
/// Specifies if the command line option is required.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredAttribute : Attribute;

/// <summary>
/// Specifies the short form of a command-line option.
/// <para/>
/// The single dash (<c>-</c>) prefix will be prepended automatically.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions('o')]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ShortOptionsAttribute : Attribute
{
    internal IEnumerable<char> Options { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ShortOptionsAttribute"/>.
    /// </summary>
    public ShortOptionsAttribute(char mainOptionName, params char[] otherOptions)
    {
        Options = otherOptions.Prepend(mainOptionName);
    }
}

/// <summary>
/// Specifies the long form of a command-line option.
/// <para/>
/// The double dash (<c>--</c>) prefix will be prepended automatically.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [LongOptions("output")]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LongOptionsAttribute : Attribute
{
    internal IEnumerable<string> Options { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="LongOptionsAttribute"/>.
    /// </summary>
    public LongOptionsAttribute(string mainOptionName, params string[] otherOptions)
    {
        Options = otherOptions.Prepend(mainOptionName);
    }
}

/// <summary>
/// Defines meaning of this command line option.
/// Used when '--help' is called.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class HelpAttribute : Attribute
{
    private readonly string _description;

    /// <summary>
    /// Creates a new instance of the <see cref="HelpAttribute"/>.
    /// </summary>
    /// <param name="description">Help description</param>
    public HelpAttribute(string description)
    {
        _description = description;
    }
}

/// <summary>
/// Allows the user to define dependencies on another options.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions("-o")]
///     public string Output { get; set; }
/// //
///     [ShortOptions("-a")]
///     [Requires(nameof(Output))]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiresAttribute : Attribute
{
    private readonly string[] _propertyName;

    /// <summary>
    /// Creates a new instance of the <see cref="RequiredAttribute"/>.
    /// </summary>
    /// <param name="propertyName">Required property names</param>
    public RequiresAttribute(params string[] propertyName)
    {
        _propertyName = propertyName;
    }
}

/// <summary>
/// Overrides the option value placeholder name used in the --help message
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ValuePlaceholderAttribute : Attribute
{
    internal string PlaceHolder { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ValuePlaceholderAttribute"/>.
    /// </summary>
    public ValuePlaceholderAttribute(string placeHolder)
    {
        PlaceHolder = placeHolder;
    }
}

/// <summary>
/// Base class for defining custom option validator attributes.
/// </summary>
/// <typeparam name="TType">Type of the option</typeparam>
[AttributeUsage(AttributeTargets.Property)]
public abstract class OptionValidatorAttribute<TType> : Attribute
    where TType : IParsable<TType>
{
    /// <summary>
    /// Performs value validation
    /// </summary>
    /// <param name="arg">The option's value</param>
    /// <param name="errorMessage">Error message describing the validation error</param>
    public abstract bool Validate(TType arg, out string? errorMessage);
}

/// <summary>
/// Base class for defining custom arguments class validator attributes.
/// </summary>
/// <typeparam name="TArgs">Type of the arguments class</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public abstract class ClassValidatorAttribute<TArgs> : Attribute
    where TArgs : BaseArgs
{
    /// <summary>
    /// Performs value validation
    /// </summary>
    /// <param name="args">The option's value</param>
    /// <param name="errorMessage">Error message describing the validation error</param>
    public abstract bool Validate(TArgs args, out string? errorMessage);
}

/// <summary>
/// Specifies the example usage string used in the --help message
/// </summary>
/// <example>
/// <code>
/// [ExampleUsage("time [options] command [arguments...]")]
/// internal sealed class TimeArgs : BaseArgs
/// {
///     [
///         ShortOptions('a'),
///         LongOptions("append"),
///         Help("(Used together with -o.) Do not overwrite but append."),
///         Requires(nameof(Output)),
///     ]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExampleUsageAttribute : Attribute
{
    internal string Usage { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ExampleUsageAttribute"/>.
    /// </summary>
    public ExampleUsageAttribute(string usage)
    {
        Usage = usage;
    }
}

/// <summary>
/// Validates if the value fits in the defined range.
/// </summary>
/// <typeparam name="T">Type of the option to compare, needs to implement IComparable{T}</typeparam>
public sealed class RangeAttribute<T> : OptionValidatorAttribute<T>
    where T : IParsable<T>, IComparable<T>
{
    private readonly T _min;
    private readonly T _max;

    /// <summary>
    /// Creates a new instance of the <see cref="RangeAttribute{T}"/>.
    /// </summary>
    /// <param name="min">Inclusive minimum of the range</param>
    /// <param name="max">Exclusive maximum of the range</param>
    public RangeAttribute(T min, T max)
    {
        _min = min;
        _max = max;
    }

    public override bool Validate(T arg, out string? errorMessage)
    {
        if (arg.CompareTo(_min) < 0)
        {
            errorMessage = $"The argument {arg} must be less than {_min}";
            return false;
        }

        if (arg.CompareTo(_max) >= 0)
        {
            errorMessage = $"The argument {arg} must be more than {_max}";
            return false;
        }

        errorMessage = null;
        return true;
    }
}
