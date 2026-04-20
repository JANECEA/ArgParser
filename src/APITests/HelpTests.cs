using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace APITests;

public class HelpTests
{
    private sealed class HelpArgs : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    public void HelpSpecifiedThrowsHelpCalledException(string args)
    {
        var parser = ArgParserFactory.FromType<HelpArgs>();

        Assert.Throws<HelpCalledException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }

    [Fact]
    public void GenerateHelpMessageReturnsText()
    {
        var parser = ArgParserFactory.FromType<HelpArgs>();

        var help = parser.GenerateHelpMessage();

        Assert.NotNull(help);
        Assert.NotEmpty(help);
    }

    private class HelpDescriptionArgs : BaseArgs
    {
        [LongNames("output")]
        [Help("Specify output file")]
        public string? OutputFile { get; set; }
        
        public override string[] PlainArguments { get; set; } = [];
    }
    
    [Fact]
    public void HelpDescriptionIsUsed()
    {
        var parser = ArgParserFactory.FromType<HelpDescriptionArgs>();
        
        var help = parser.GenerateHelpMessage();
        
        Assert.Contains("Specify output file", help);
    }
    
    private class MetaVarDescrArgs : BaseArgs
    {
        [LongNames("output")]
        [MetaVarName("FILE")]
        public string? OutputFile { get; set; }
        
        public override string[] PlainArguments { get; set; } = [];
    }
    
    [Fact]
    public void MetaVarNameIsUsed()
    {
        var parser = ArgParserFactory.FromType<MetaVarDescrArgs>();
        
        var help = parser.GenerateHelpMessage();
        
        Assert.Contains("FILE", help);
    }
}