using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

#pragma warning disable xUnit1026
namespace Tests.ArgParserTests;

public class SimpleTests
{
    [ExampleUsage("program [options]")]
    internal sealed class SimpleArgs_PrimitiveTypes_OptionalOnlyOptions : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];

        [
            ShortNames('i', 'y', 'n'),
            LongNames("int", "alpha", "bravo"),
            Help("Integer option"),
            MetaVarName("INT_VALUE")
        ]
        public int? IntegerOption { get; set; }

        [
            ShortNames('s', 'g'),
            LongNames("string", "text", "veryveryverylongoptionname"),
            Help("String option"),
            MetaVarName("STR_VALUE")
        ]
        public string? StringOption { get; set; }

        [ShortNames('b', 'c', 'd'), LongNames("boooolean", "oo", "bool"), Help("Boolean flag")]
        public bool BoolOption { get; set; }
    }

    [Theory]
    [InlineData(new[] { "-i", "7" }, 7)]
    [InlineData(new[] { "-i", "0" }, 0)]
    [InlineData(new[] { "-i", "-14" }, -14)]
    [InlineData(new[] { "-i", "1000000000" }, 1_000_000_000)]
    [InlineData(new[] { "-i", "2147483647" }, 2147483647)]
    [InlineData(new[] { "-i", "-2147483648" }, -2147483648)]
    [InlineData(new[] { "-i", "100" }, 100)]
    [InlineData(new[] { "-y", "8" }, 8)]
    [InlineData(new[] { "-n", "8" }, 8)]
    [InlineData(new[] { "-i=7" }, 7)]
    [InlineData(new[] { "--int=7" }, 7)]
    [InlineData(new[] { "--int", "7" }, 7)]
    [InlineData(new[] { "--int=0" }, 0)]
    [InlineData(new[] { "--int=-14" }, -14)]
    [InlineData(new[] { "--int=1000000000" }, 1_000_000_000)]
    [InlineData(new[] { "--int=2147483647" }, 2147483647)]
    [InlineData(new[] { "--int=-2147483648" }, -2147483648)]
    [InlineData(new[] { "--int=100" }, 100)]
    [InlineData(new[] { "--alpha=8" }, 8)]
    [InlineData(new[] { "--bravo=8" }, 8)]
    public void IntTypeParsing_Optional_ValidInput(string[] args, int? Expected_IntegerOptionResult)
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();
        var result = parser.Parse(args);

        Assert.Equal(Expected_IntegerOptionResult, result.IntegerOption);
    }

    public static IEnumerable<object[]> InvalidIntInputs =>
        [
            [new[] { "-i", "stringValue" }],
            [new[] { "-i" }],
            [new[] { "-y" }],
            [new[] { "-n" }],
            [new[] { "--int=ahoj" }],
            [new[] { "--alpha=ahoj" }],
            [new[] { "--bravo=ahoj" }],
            [new[] { "--int=77a" }],
            [new[] { "--int=\"\"" }],
            [new[] { "--int" }],
            [new[] { "--alpha" }],
            [new[] { "--bravo" }],
        ];

    [Theory]
    [MemberData(nameof(InvalidIntInputs))]
    public void IntTypeParsing_Optional_InvalidInput(string[] args)
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();

        Assert.ThrowsAny<CommandLineParsingException>(() =>
        {
            var result = parser.Parse(args);
        });
    }

    [Theory]
    [InlineData(new[] { "-s", "s" }, "s")]
    [InlineData(new[] { "-s", "abc" }, "abc")]
    [InlineData(new[] { "-s", "veryverylong" }, "veryverylong")]
    [InlineData(new[] { "-s", "gh_@_$!?<>special" }, "gh_@_$!?<>special")]
    [InlineData(new[] { "-s", "\"Multi token string\"" }, "\"Multi token string\"")]
    [InlineData(new[] { "-s", "\"\"" }, "\"\"")]
    [InlineData(new[] { "-s", "\"   \"" }, "\"   \"")]
    [InlineData(new[] { "-g", "text" }, "text")]
    [InlineData(new[] { "--string=s" }, "s")]
    [InlineData(new[] { "--string=abc" }, "abc")]
    [InlineData(new[] { "--string=veryverylong" }, "veryverylong")]
    [InlineData(new[] { "--string=gh_@_$!?<>special" }, "gh_@_$!?<>special")]
    [InlineData(new[] { "--string=\"Multi token string\"" }, "\"Multi token string\"")]
    [InlineData(new[] { "--string=\"\"" }, "\"\"")]
    [InlineData(new[] { "--string=\"   \"" }, "\"   \"")]
    [InlineData(new[] { "--text=something" }, "something")]
    [InlineData(new[] { "--veryveryverylongoptionname=something" }, "something")]
    [InlineData(new[] { "-s", "-g" }, "-g")]
    public void StringTypeParsing_Optional_ValidInput(
        string[] args,
        string? Expected_StringOptionResult
    )
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();
        var result = parser.Parse(args);

        Assert.Equal(Expected_StringOptionResult, result.StringOption);
    }

    public static IEnumerable<object[]> InvalidStringInputs =>
        [
            [new[] { "-s" }],
            [new[] { "-g" }],
            [new[] { "--string" }],
            [new[] { "--text" }],
            [new[] { "--veryveryverylongoptionname" }],
        ];

    [Theory]
    [MemberData(nameof(InvalidStringInputs))]
    public void StringTypeParsing_Optional_InvalidInput(string[] args)
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();

        Assert.Throws<MissingOptionValueException>(() =>
        {
            var result = parser.Parse(args);
        });
    }

    [Theory]
    [InlineData(new string[0], false)]
    [InlineData(new[] { "-b" }, true)]
    [InlineData(new[] { "-c" }, true)]
    [InlineData(new[] { "-d" }, true)]
    [InlineData(new[] { "--boooolean" }, true)]
    [InlineData(new[] { "--bool" }, true)]
    [InlineData(new[] { "--oo" }, true)]
    public void BoolTypeParsing_Optional_ValidInput(string[] args, bool Expected_BoolOptionResult)
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();
        var result = parser.Parse(args);

        Assert.Equal(Expected_BoolOptionResult, result.BoolOption);
    }

    [Theory]
    [InlineData(new[] { "-i", "8" }, 8, null, false)]
    [InlineData(new[] { "-y", "8" }, 8, null, false)]
    [InlineData(new[] { "-n", "8" }, 8, null, false)]
    [InlineData(new[] { "--int=8" }, 8, null, false)]
    [InlineData(new[] { "--alpha=8" }, 8, null, false)]
    [InlineData(new[] { "--bravo=8" }, 8, null, false)]
    [InlineData(new[] { "-s", "abc" }, null, "abc", false)]
    [InlineData(new[] { "-g", "abc" }, null, "abc", false)]
    [InlineData(new[] { "--string=abc" }, null, "abc", false)]
    [InlineData(new[] { "--text=abc" }, null, "abc", false)]
    [InlineData(new[] { "--veryveryverylongoptionname=abc" }, null, "abc", false)]
    [InlineData(new[] { "-b" }, null, null, true)]
    [InlineData(new[] { "-c" }, null, null, true)]
    [InlineData(new[] { "-d" }, null, null, true)]
    [InlineData(new[] { "--boooolean" }, null, null, true)]
    [InlineData(new[] { "--bool" }, null, null, true)]
    [InlineData(new[] { "--oo" }, null, null, true)]
    [InlineData(new[] { "-i", "8", "-s", "abc" }, 8, "abc", false)]
    [InlineData(new[] { "-i", "8", "-s", "abc", "-b" }, 8, "abc", true)]
    [InlineData(new[] { "--int=8", "-s", "abc", "-b" }, 8, "abc", true)]
    [InlineData(new[] { "-i", "8", "--string=abc", "-b" }, 8, "abc", true)]
    [InlineData(new[] { "-i", "8", "-s", "abc", "--boooolean" }, 8, "abc", true)]
    [InlineData(new[] { "--int=8", "--string=abc", "--boooolean" }, 8, "abc", true)]
    public void PrimitiveTypes_AllOptional_ValidInput(
        string[] args,
        int? Expected_IntegerOptionResult,
        string? Expected_StringOptionResult,
        bool Expected_BoolOptionResult
    )
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();
        var result = parser.Parse(args);

        Assert.Equal(Expected_IntegerOptionResult, result.IntegerOption);
        Assert.Equal(Expected_StringOptionResult, result.StringOption);
        Assert.Equal(Expected_BoolOptionResult, result.BoolOption);
    }

    [Theory]
    [InlineData(new[] { "-s", "abc", "-i", "8" }, 8, "abc", false)]
    [InlineData(new[] { "-s", "abc", "-i", "8", "-b" }, 8, "abc", true)]
    [InlineData(new[] { "-i", "8", "-b", "-s", "abc" }, 8, "abc", true)]
    [InlineData(new[] { "-s", "abc", "-b", "-i", "8" }, 8, "abc", true)]
    [InlineData(new[] { "-b", "-i", "8", "-s", "abc" }, 8, "abc", true)]
    [InlineData(new[] { "-b", "-s", "abc", "-i", "8" }, 8, "abc", true)]
    public void PrimitiveTypes_ShuffledOptions_ValidInput(
        string[] args,
        int? Expected_IntegerOptionResult,
        string? Expected_StringOptionResult,
        bool Expected_BoolOptionResult
    )
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();
        var result = parser.Parse(args);

        Assert.Equal(Expected_IntegerOptionResult, result.IntegerOption);
        Assert.Equal(Expected_StringOptionResult, result.StringOption);
        Assert.Equal(Expected_BoolOptionResult, result.BoolOption);
    }

    [Theory]
    [InlineData(new[] { "-i", "8", "-s", "abc", "-b", "plain1" }, new[] { "plain1" })]
    [InlineData(new[] { "plain1", "plain2" }, new[] { "plain1", "plain2" })]
    [InlineData(new[] { "-b", "plain1" }, new[] { "plain1" })]
    [InlineData(new[] { "-s", "plain1", "plain2" }, new[] { "plain2" })]
    [InlineData(new[] { "--string=plain1", "plain2" }, new[] { "plain2" })]
    public void PlainArguments_NoSeparator_ValidInput(string[] args, string[] expectedPlain)
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();
        var result = parser.Parse(args);
        Assert.Equal(expectedPlain, result.PlainArguments);
    }

    public static IEnumerable<object[]> InvalidCLIInputs =>
        [
            [new[] { "-i", "-s", "abc" }],
            [new[] { "--neexistuje=8" }],
            [new[] { "-i", "5", "-i", "6" }],
            [new[] { "-i", "5", "-y", "6" }],
            [new[] { "--int=8", "--int=9", "--alpha=10" }],
        ];

    [Theory]
    [MemberData(nameof(InvalidCLIInputs))]
    public void InvalidOptionsSet(string[] args)
    {
        var parser = ArgParserFactory.FromType<SimpleArgs_PrimitiveTypes_OptionalOnlyOptions>();
        Assert.ThrowsAny<CommandLineParsingException>(() =>
        {
            var result = parser.Parse(args);
        });
    }
}
