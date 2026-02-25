using ArgParser.Attributes;

namespace ArgParser;

public abstract class BaseArgs
{
    [ShortOptions("-h")]
    [LongOptions("--help")]
    public virtual bool HelpCalled { get; set; }

    public string[] PlainArguments { get; set; }
}

public static class ArgParserFactory
{
    public static ArgParser<TArgs> FromType<TArgs>()
        where TArgs : BaseArgs
    {
        throw new NotImplementedException();
    }
}

public class ArgParser<T>
    where T : BaseArgs
{
    internal ArgParser() { }

    public T Parse(string[] args)
    {
        throw new NotImplementedException();
    }
}
