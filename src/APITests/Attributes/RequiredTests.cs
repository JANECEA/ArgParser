using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace APITests.Attributes;

public static class RequiredTests
{
    private class RequiredFlagArg : BaseArgs
    {
        [Required]
        public bool Flag { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void RequiredFlagThrowsDuringCreation()
    {
        Assert.Throws<RequiredOnFlagException>(ArgParserFactory.FromType<RequiredFlagArg>);
    }

    private class SimpleRequired : BaseArgs
    {
        [ShortNames('a')]
        [Required]
        public string? Value { get; set; }

        [ShortNames('i')]
        public int? IntValue { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("")]
    [InlineData("-i 123")]
    public static void ThrowsOnUnsetRequired(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleRequired>();

        Assert.Throws<MissingRequiredOptionException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [Theory]
    [InlineData("-a")]
    [InlineData("-a=")]
    public static void ThrowsOnEmptyRequired(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleRequired>();

        Assert.Throws<MissingOptionValueException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [Fact]
    public static void DoesNotThrowOnSetRequired()
    {
        var parser = ArgParserFactory.FromType<SimpleRequired>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs("-a=val"));

        Assert.Equal("val", result.Value);
    }

    private class MultipleRequiredArgs : BaseArgs
    {
        [ShortNames('a')]
        [Required]
        public string? ValueA { get; set; }

        [ShortNames('b')]
        [Required]
        public string? ValueB { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("-a AValue")]
    [InlineData("-b BValue")]
    [InlineData("nothing set")]
    public static void ThrowsOnNotAllRequiredSet(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleRequired>();

        Assert.Throws<MissingRequiredOptionException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }
}
