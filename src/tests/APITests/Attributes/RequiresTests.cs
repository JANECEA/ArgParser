using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.APITests.Attributes;

public static class RequiresTests
{
    private class IntRequiresStringArgs : BaseArgs
    {
        [ShortNames('s')]
        public string? Value { get; set; }

        [ShortNames('i')]
        [Requires(nameof(Value))]
        public int? IntValue { get; set; }

        [ShortNames('b')]
        public bool BoolValue { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("")]
    [InlineData("-b")]
    [InlineData("-s banana")]
    public static void DoesNotThrowOnUnspecifiedDependency(string args)
    {
        var parser = ArgParserFactory.FromType<IntRequiresStringArgs>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        // no assert, we just don't want an exception to be thrown here
    }

    [Theory]
    [InlineData("-i 123")]
    [InlineData("-b -i 123")]
    [InlineData("-i 123 -b")]
    public static void ThrowsOnMissingDependency(string args)
    {
        var parser = ArgParserFactory.FromType<IntRequiresStringArgs>();

        // could not find sealed exception for this case -> keep it abstract
        Assert.ThrowsAny<CommandLineParsingException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [Theory]
    [InlineData("-s banana -i 123")]
    // this behavior is not well-documented, so my opinion is that it should not throw
    [InlineData("-i 123 -s banana")]
    public static void CapturedOnSpecifiedDependency(string args)
    {
        var parser = ArgParserFactory.FromType<IntRequiresStringArgs>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(123, result.IntValue);
    }

    private class DoubleDependencyArgs : BaseArgs
    {
        [ShortNames('s')]
        public string? Value { get; set; }

        [ShortNames('i')]
        [Requires(nameof(Value))]
        public int? IntValue1 { get; set; }

        [ShortNames('j')]
        [Requires(nameof(IntValue1))]
        public int? IntValue2 { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("-s banana -i 123 -j 321")]
    [InlineData("-s banana -j 321 -i 123")]
    [InlineData("-i 123 -s banana -j 321")]
    [InlineData("-i 123 -j 321 -s banana")]
    [InlineData("-j 321 -i 123 -s banana")]
    [InlineData("-j 321 -s banana -i 123")]
    public static void CaptureOnAllDependenciesSet(string args)
    {
        var parser = ArgParserFactory.FromType<DoubleDependencyArgs>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        // This is not the main point of this test, but it is a good sanity check.
        Assert.Equal("banana", result.Value);
        Assert.Equal(123, result.IntValue1);
        Assert.Equal(321, result.IntValue2);
    }

    [Theory]
    [InlineData("-s banana -j 321")]
    [InlineData("-j 321")]
    public static void ThrowsOnUnspecifiedAllDependencies(string args)
    {
        var parser = ArgParserFactory.FromType<DoubleDependencyArgs>();

        Assert.ThrowsAny<CommandLineParsingException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    // Note that testing correctly thrown ReferencedOptionNotFoundException during parsing
    // is not possible, since it gets caught already by the compiler. E.g., the class bellow:
    /*
    private class Args : BaseArgs
    {
        [ShortNames('a')]
        [Requires(nameof(Output))]
        public bool Append { get; set; }
        
        public override string[] PlainArguments { get; set; } = [];
    }
    */
}
