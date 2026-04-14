using ArgParser.Analyzers.Abstractions;
using ArgParser.Exceptions;

namespace ArgParser.Attributes;

internal interface IOptionValidator
{
    internal Type ValidatorType { get; }

    internal bool ValidateInternal(object arg, out string? errorMessage);
}

/// <summary>
/// Base class for defining custom option validator attributes.
/// </summary>
/// <typeparam name="TType">Type of the option</typeparam>
[AttributeUsage(AttributeTargets.Property)]
public abstract class OptionValidatorAttribute<TType>
    : Attribute,
        IOptionValidator,
        IOnParsable,
        IOnPropertyType<TType>
    where TType : IParsable<TType>
{
    public Type ValidatorType => typeof(TType);

    bool IOptionValidator.ValidateInternal(object arg, out string? errorMessage) =>
        Validate((TType)arg, out errorMessage);

    /// <summary>
    /// Performs value validation
    /// </summary>
    /// <param name="arg">The option's value</param>
    /// <param name="errorMessage">Error message describing the validation error</param>
    public abstract bool Validate(TType arg, out string? errorMessage);
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

        if (_min.CompareTo(max) >= 0)
            throw new ParserConfigurationException($"The argument {min} must be less than {_max}");
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

/// <summary>
/// Base class for defining custom arguments class validator attributes.
/// </summary>
/// <typeparam name="TArgs">Type of the arguments class</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public abstract class ClassValidatorAttribute<TArgs> : Attribute, IOnClassType<BaseArgs>
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
/// Specifies whether this program accepts plain arguments.
/// <br/>
/// Allowed by default.
/// </summary>
public sealed class AllowPlainArgumentsAttribute : ClassValidatorAttribute<BaseArgs>
{
    internal bool Allow { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="AllowPlainArgumentsAttribute"/>.
    /// </summary>
    public AllowPlainArgumentsAttribute(bool allow)
    {
        Allow = allow;
    }

    public override bool Validate(BaseArgs args, out string? errorMessage)
    {
        if (!Allow && args.PlainArguments?.Length > 0)
        {
            errorMessage = "Plain arguments are not allowed.";
            return false;
        }
        errorMessage = null;
        return true;
    }
}
