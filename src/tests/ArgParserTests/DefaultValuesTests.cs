using ArgParser;
using ArgParser.Attributes;

namespace Tests.ArgParserTests;

public class DefaultValuesTests
{
    [ExampleUsage("program [options]")]
    internal class DefaultPrimitiveOptions : BaseArgs
    {
        [ShortNames('i')]
        public int? Int { get; set; } = 8;

        [ShortNames('s')]
        public string? Str { get; set; } = "ahoj";
        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData(new string[] { }, 8, "ahoj")]
    [InlineData(new[] { "-i", "9" }, 9, "ahoj")]
    [InlineData(new[] { "-s", "abc" }, 8, "abc")]
    public void DafaultValue(string[] args, int? expectedInt, string? expectedStr)
    {
        var parser = ArgParserFactory.FromType<DefaultPrimitiveOptions>();
        var result = parser.Parse(args);
        Assert.Equal(expectedInt, result.Int);
        Assert.Equal(expectedStr, result.Str);
    }
}
