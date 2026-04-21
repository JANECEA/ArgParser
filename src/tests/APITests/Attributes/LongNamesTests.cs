using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.APITests.Attributes;

public static class LongNamesTests
{
    [AllowPlainArguments(true)]
    private class SimpleLongsCase : BaseArgs
    {
        [LongNames("value")]
        public string? Value { get; set; }

        [LongNames("intvalue")]
        public int? IntValue { get; set; }

        [LongNames("boolvalue")]
        public bool BoolValue { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("--value val", "val")]
    [InlineData("--value v", "v")]
    [InlineData("--value 123", "123")]
    [InlineData("--value val val", "val")]
    [InlineData("--value=val", "val")]
    [InlineData("--value=v", "v")]
    [InlineData("--value=val val", "val")]
    public static void StringCapture(string args, string? expected)
    {
        var parser = ArgParserFactory.FromType<SimpleLongsCase>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData("--value")]
    [InlineData("--value=")]
    public static void MissingValueThrows(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleLongsCase>();

        Assert.Throws<MissingOptionValueException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [Theory]
    [InlineData("--intvalue 123", 123)]
    [InlineData("--intvalue 123 123", 123)]
    [InlineData("--intvalue -123", -123)]
    [InlineData("--intvalue 0", 0)]
    [InlineData("--intvalue 2147483647", 2147483647)]
    [InlineData("--intvalue -2147483648", -2147483648)]
    [InlineData("--intvalue=123", 123)]
    [InlineData("--intvalue=123 123", 123)]
    [InlineData("--intvalue=-123", -123)]
    [InlineData("--intvalue=0", 0)]
    [InlineData("--intvalue=2147483647", 2147483647)]
    [InlineData("--intvalue=-2147483648", -2147483648)]
    public static void IntCapture(string args, int? expected)
    {
        var parser = ArgParserFactory.FromType<SimpleLongsCase>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expected, result.IntValue);
    }

    [Theory]
    [InlineData("--intvalue 2147483648")]
    [InlineData("--intvalue abc")]
    [InlineData("--intvalue 12.3")]
    public static void InvalidIntValueThrows(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleLongsCase>();

        Assert.Throws<ValueParsingException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }

    [Theory]
    [InlineData("--boolvalue", true)]
    [InlineData("--boolvalue true false", true)]
    [InlineData("--boolvalue=true false", true)]
    [InlineData("", false)]
    [InlineData("--boolvalue false", false)]
    [InlineData("--boolvalue=false", false)]
    public static void BoolCapture(string args, bool expected)
    {
        var parser = ArgParserFactory.FromType<SimpleLongsCase>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expected, result.BoolValue);
    }

    [Theory]
    [InlineData("--nonexistent 123")]
    [InlineData("--nonexistent")]
    [InlineData("--nonexistent=")]
    [InlineData("--nonexistent val")]
    public static void NonExistentLongNameThrows(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleLongsCase>();
        Assert.Throws<UnknownOptionException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }

    private class MultipleLongNames : BaseArgs
    {
        [LongNames("ss", "ii", "jj")]
        public string? Value { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("--ss val", "val")]
    [InlineData("--ii val", "val")]
    [InlineData("--jj val", "val")]
    public static void CaptureWithAliasing(string args, string? expected)
    {
        var parser = ArgParserFactory.FromType<MultipleLongNames>();
        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));
        Assert.Equal(expected, result.Value);
    }

    class ConflictingArgs : BaseArgs
    {
        [LongNames("app")]
        public bool Append { get; set; }

        [LongNames("app")]
        public bool Allow { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void ConflictingLongNamesThrows()
    {
        Assert.Throws<DuplicateOptionNameException>(ArgParserFactory.FromType<ConflictingArgs>);
    }

    class InvalidLongNameArgs : BaseArgs
    {
        [LongNames("append option")]
        public bool Append { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void InvalidLongNameThrows()
    {
        Assert.Throws<IncorrectNameFormatException>(ArgParserFactory.FromType<InvalidLongNameArgs>);
    }
}
