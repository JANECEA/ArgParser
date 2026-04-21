using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace APITests.Attributes;

public static class ShortNamesTests
{
    [AllowPlainArguments(true)]
    private class SimpleShortsCase : BaseArgs
    {
        [ShortNames('s')]
        public string? Value { get; set; }

        [ShortNames('i')]
        public int? IntValue { get; set; }

        [ShortNames('b')]
        public bool BoolValue { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("-s val", "val")]
    [InlineData("-s v", "v")]
    [InlineData("-s 123", "123")]
    [InlineData("-s val val", "val")]
    [InlineData("-s=val", "val")]
    [InlineData("-s=v", "v")]
    [InlineData("-s=val val", "val")]
    public static void StringCapture(string args, string? expected)
    {
        var parser = ArgParserFactory.FromType<SimpleShortsCase>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData("-s")]
    [InlineData("-s=")]
    public static void MissingStringValueThrows(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleShortsCase>();

        Assert.Throws<MissingOptionValueException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [Theory]
    [InlineData("-i 123", 123)]
    [InlineData("-i 123 123", 123)]
    [InlineData("-i -123", -123)]
    [InlineData("-i 0", 0)]
    [InlineData("-i 2147483647", 2147483647)] // Max Int32
    [InlineData("-i -2147483648", -2147483648)] // Min Int32
    [InlineData("-i=123", 123)]
    [InlineData("-i=123 123", 123)]
    [InlineData("-i=-123", -123)]
    [InlineData("-i=0", 0)]
    [InlineData("-i=2147483647", 2147483647)] // Max Int32
    [InlineData("-i=-2147483648", -2147483648)] // Min Int32
    public static void IntCapture(string args, int? expected)
    {
        var parser = ArgParserFactory.FromType<SimpleShortsCase>();
        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expected, result.IntValue);
    }

    [Theory]
    [InlineData("-i")]
    [InlineData("-i=")]
    public static void MissingIntValueThrows(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleShortsCase>();

        Assert.Throws<MissingOptionValueException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [Theory]
    [InlineData("-i 2147483648")] // Overflow (Max + 1)
    [InlineData("-i abc")]
    [InlineData("-i 12.3")]
    public static void InvalidIntValueThrows(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleShortsCase>();

        Assert.Throws<ValueParsingException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }

    // I did not find anywhere whether this specification of flags is possible
    // I would prefer it, and therefore, it is in the tests
    [Theory]
    [InlineData("-b", true)]
    [InlineData("-b true false", true)]
    [InlineData("-b=true false", true)]
    [InlineData("", false)]
    [InlineData("-b false", false)]
    [InlineData("-b=false", false)]
    public static void BoolCapture(string args, bool expected)
    {
        var parser = ArgParserFactory.FromType<SimpleShortsCase>();
        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expected, result.BoolValue);
    }

    [Theory]
    [InlineData("-k 123")]
    [InlineData("-k")]
    [InlineData("-k=")]
    [InlineData("-k val")]
    public static void NonExistentShortNameThrows(string args)
    {
        var parser = ArgParserFactory.FromType<SimpleShortsCase>();
        Assert.Throws<UnknownOptionException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }

    private class MultipleShortNames : BaseArgs
    {
        [ShortNames('s', 'i', 'j')]
        public string? Value { get; set; }
        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("-s val", "val")]
    [InlineData("-i val", "val")]
    [InlineData("-j val", "val")]
    public static void CaptureWithAliasing(string args, string? expected)
    {
        var parser = ArgParserFactory.FromType<MultipleShortNames>();
        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));
        Assert.Equal(expected, result.Value);
    }

    class ConflictingArgs : BaseArgs
    {
        [ShortNames('a')]
        public bool Append { get; set; }

        [ShortNames('a')]
        public bool Allow { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void ConflictingShortNamesThrows()
    {
        Assert.Throws<DuplicateOptionNameException>(ArgParserFactory.FromType<ConflictingArgs>);
    }
}
