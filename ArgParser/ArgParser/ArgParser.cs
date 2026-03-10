using ArgParser.Attributes;

namespace ArgParser;

public abstract class BaseArgs
{
    [ShortOptions('h')]
    [LongOptions("help")]
    public virtual bool HelpCalled { get; set; }

    public abstract string[] PlainArguments { get; set; }
}

public static class ArgParserFactory
{
    public static ArgParser<TArgs> FromType<TArgs>()
        where TArgs : BaseArgs
    {
        return new ArgParser<TArgs>();
    }
}

public sealed class ArgParser<TArgs>
    where TArgs : BaseArgs
{
    internal ArgParser() { }

    public TArgs Parse(string[] args)
    {
        throw new NotImplementedException();
    }
}
