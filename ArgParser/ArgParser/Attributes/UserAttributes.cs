namespace ArgParser.Attributes;

/// <summary>
/// Specifies if the command line option is required.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredAttribute : Attribute;

/// <summary>
/// Specifies the short form of a command-line option.
/// <para/>
/// The short name should have a single dash (<c>-</c>) prefix.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [ShortOptions("-o")]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ShortOptionsAttribute : Attribute
{
    private readonly string[] _options;

    /// <summary>
    /// Creates a new instance of the <see cref="ShortOptionsAttribute"/>.
    /// </summary>
    /// <param name="options">Short option names</param>
    public ShortOptionsAttribute(params string[] options)
    {
        _options = options;
    }
}

/// <summary>
/// Specifies the long form of a command-line option.
/// <para/>
/// The short name should have a double dash (<c>--</c>) prefix.
/// </summary>
/// <example>
/// <code>
/// class Args : BaseArgs
/// {
///     [LongOptions("--output")]
///     public string Output { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LongOptionsAttribute : Attribute
{
    private readonly string[] _options;

    /// <summary>
    /// Creates a new instance of the <see cref="LongOptionsAttribute"/>.
    /// </summary>
    /// <param name="options">Long option names</param>
    public LongOptionsAttribute(params string[] options)
    {
        _options = options;
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
/// This attribute allows the user to register options as mutually exclusive.
/// </summary>
/// <example>
/// <code>
/// [MutuallyExclusive(nameof(Truncate), nameof(Append))]
/// class Args : BaseArgs
/// {
///     [ShortOptions("-t")]
///     public bool Truncate { get; set; }
/// //
///     [ShortOptions("-a")]
///     public bool Append { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MutuallyExclusiveAttribute : Attribute
{
    internal IEnumerable<string> propertyNames { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="MutuallyExclusiveAttribute"/>.
    /// </summary>
    public MutuallyExclusiveAttribute(
        string propertyNameA,
        string propertyNameB,
        params string[] otherPropertyNames
    )
    {
        propertyNames = otherPropertyNames.Prepend(propertyNameB).Prepend(propertyNameA);
    }
}

/// <summary>
/// Base class for defining custom option validator attributes.
/// </summary>
/// <typeparam name="TType">Type of the option</typeparam>
[AttributeUsage(AttributeTargets.Property)]
public abstract class ValidatorAttribute<TType> : Attribute
    where TType : IParsable<TType>
{
    /// <summary>
    /// Performs value validation
    /// </summary>
    /// <param name="arg">The option's value</param>
    /// <param name="errorMessage"></param>
    public abstract bool Validate(TType arg, out string? errorMessage);
}

/// <summary>
/// Validates if the value fits in the defined range.
/// </summary>
/// <typeparam name="T">Type of the option to compare, needs to implement IComparable{T}</typeparam>
public sealed class RangeAttribute<T> : ValidatorAttribute<T>
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
