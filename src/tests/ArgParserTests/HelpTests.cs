using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.ArgParserTests;

public class HelpTests
{
    [ExampleUsage("program [options]")]
    internal sealed class HelpOptions : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];

        [ShortNames('i'), LongNames("int"), Help("Integer option"), MetaVarName("INT_VALUE")]
        public int? IntegerOption { get; set; }

        [ShortNames('s'), LongNames("string"), Help("String option"), MetaVarName("STR_VALUE")]
        public string? StringOption { get; set; }
    }

    [Fact]
    public void HelpThrown()
    {
        var parser = ArgParserFactory.FromType<HelpOptions>();
        Assert.Throws<HelpCalledException>(() => parser.Parse(["--help"]));
    }

    [Fact]
    public void HelpMessage_ContainsExampleUsage()
    {
        var parser = ArgParserFactory.FromType<HelpOptions>();
        var help = parser.GenerateHelpMessage();
        Assert.Contains("program [options]", help);
    }

    [Fact]
    public void HelpMessage_ContainsHelpTexts()
    {
        var parser = ArgParserFactory.FromType<HelpOptions>();
        var help = parser.GenerateHelpMessage();
        Assert.Contains("Integer option", help);
        Assert.Contains("String option", help);
    }

    [Fact]
    public void HelpMessage_ContainsMetaVarNames()
    {
        var parser = ArgParserFactory.FromType<HelpOptions>();
        var help = parser.GenerateHelpMessage();
        Assert.Contains("INT_VALUE", help);
        Assert.Contains("STR_VALUE", help);
    }
}
