namespace ArgParser;

[AttributeUsage(AttributeTargets.Property)]
public class ShortOptionsAttribute : Attribute
{
    public ShortOptionsAttribute(params string[] options)
    {
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class LongOptionsAttribute : Attribute
{
    public LongOptionsAttribute(params string[] options)
    {
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class HelpAttribute : Attribute
{
    public HelpAttribute(string description)
    {
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RequireAttribute : Attribute
{
    public RequireAttribute(string propertyName)
    {
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RangeAttribute : Attribute
{
    public RangeAttribute(int min, int max)
    {
        throw new NotImplementedException();
    }
}

public abstract class BaseArgs
{
    [ShortOptions("-h")]
    [LongOptions("--help")]
    public virtual bool HelpCalled { get; set; }

    public string[] PlainArguments { get; set; }
}

public class Parser<T>
    where T : BaseArgs
{
    public T Parse(string[] args)
    {
        throw new NotImplementedException();
    }
}
