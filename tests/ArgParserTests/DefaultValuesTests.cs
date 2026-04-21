using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;


namespace ArgParserTests
{
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
        [InlineData(new[] {"-i","-s"},8,"ahoj")]
        [InlineData(new[] { "-i","9", "-s" }, 9, "ahoj")]
        [InlineData(new[] { "-i", "-s", "abc" }, 8, "abc")]
        [InlineData(new[] { "-i" }, 8, null)]
        public void DafaultValue(string[] args, int? expectedInt, string? expectedStr)
        {
            var parser = ArgParserFactory.FromType<DefaultPrimitiveOptions>();
            var result = parser.Parse(args);
            Assert.Equal(expectedInt,result.Int);
            Assert.Equal(expectedStr, result.Str);
        }

    }
}
