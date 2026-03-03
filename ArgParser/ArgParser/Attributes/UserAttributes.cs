namespace ArgParser.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property)]
public class ShortOptionsAttribute : Attribute
{
    private readonly string[] _options;

    public ShortOptionsAttribute(params string[] options)
    {
        _options = options;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class LongOptionsAttribute : Attribute
{
    private readonly string[] _options;

    public LongOptionsAttribute(params string[] options)
    {
        _options = options;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class HelpAttribute : Attribute
{
    private readonly string _description;

    public HelpAttribute(string description)
    {
        _description = description;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RequiresAttribute : Attribute
{
    private readonly string _propertyName;

    public RequiresAttribute(string propertyName)
    {
        _propertyName = propertyName;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public abstract class ValidatorAttribute<TType> : Attribute
    where TType : IParsable<TType>
{
    public abstract string ErrorMessage { get; }

    public abstract bool IsValid(TType? arg);
}

public class RangeAttribute<T> : ValidatorAttribute<T>
    where T : IParsable<T>, IComparable<T>
{
    private readonly T _min;
    private readonly T _max;

    public override string ErrorMessage { get; }

    public RangeAttribute(T min, T max)
    {
        _min = min;
        _max = max;
    }

    public override bool IsValid(T? arg)
    {
        throw new NotImplementedException();
    }
}
