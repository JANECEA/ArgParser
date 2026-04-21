using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

#pragma warning disable xUnit1026
namespace Tests.ArgParserTests;

public class EnumTests
{
    [EnumCasePolicy(EnumCase.PreserveCase)]
    public enum Days
    {
        mon = 0,
        tue = 1,
        wen = 2,
        thu = -3,
        fri = int.MaxValue,
        sat = int.MinValue,
        son,
    }

    [ExampleUsage("program [options]")]
    internal class EnumOptions : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];

        [ShortNames('d'), LongNames("day"), Help("Enum option"), MetaVarName("ENUM_VALUE")]
        public Days? Day { get; set; }

        [
            ShortNames('s'),
            LongNames("string", "Day"),
            Help("String option"),
            MetaVarName("STRING_VALUE")
        ]
        public string? StringVal { get; set; }
    }

    [Theory]
    [InlineData(new[] { "-d", "mon" }, Days.mon)]
    [InlineData(new[] { "-d", "thu" }, Days.thu)]
    [InlineData(new[] { "-d", "fri" }, Days.fri)]
    [InlineData(new[] { "--day=son" }, Days.son)]
    [InlineData(new[] { "--day=\"son\"" }, Days.son)]
    [InlineData(new[] { "-d", "wen", "tue" }, Days.wen)]
    [InlineData(new[] { "-s", "wen", "-d", "tue" }, Days.tue)]
    [InlineData(new[] { "-d", "tue", "-s", "thu" }, Days.tue)]
    [InlineData(new[] { "--day=mon", "--Day=tue" }, Days.mon)]
    public void EnumParsing_ValidInput(string[] args, Days expected)
    {
        var parser = ArgParserFactory.FromType<EnumOptions>();
        var result = parser.Parse(args);

        Assert.Equal(expected, result.Day);
    }

    public static IEnumerable<object[]> InvalidInputs =>
        [
            [new[] { "-d" }],
            [new[] { "--day" }],
            [new[] { "-d", "0" }],
            [new[] { "-d", "dec" }],
            [new[] { "--day=dec" }],
            [new[] { "--day=" }],
        ];

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public void EnumParsing_InvalidInput(string[] args)
    {
        var parser = ArgParserFactory.FromType<EnumOptions>();

        Assert.Throws<CommandLineParsingException>(() => parser.Parse(args));
    }
}
