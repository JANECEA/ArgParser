using ArgParser;
using ArgParser.Attributes;

#pragma warning disable xUnit1026
#pragma warning disable CS0219 
namespace ArgParserTests
{
    public class RequiredOptionsTests
    {
        [ExampleUsage("program [options]")]
        class RequiredOptionsA : BaseArgs
        {
            public override string[] PlainArguments { get; set; } = [];

            [ShortNames('r','e'), LongNames("required","optionen"),Required]
            public int? RequiredInt { get; set; }


        }
        [ExampleUsage("program [options]")]
        class RequiredOptionsB : BaseArgs
        {
            public override string[] PlainArguments { get; set; } = [];

            [ShortNames('r', 'e'), LongNames("required", "optionen"), Required]
            public int? RequiredIntA { get; set; }

            [ShortNames('i', 'n'), LongNames("int"), Required]
            public int? RequiredIntB { get; set; }


        }

        [Theory]
        [InlineData(new[] {"-r","0"}, 0)]
        [InlineData(new[] { "-e", "0" }, 0)]
        [InlineData(new[] { "--required=0" }, 0)]
        [InlineData(new[] { "--optionen=0" }, 0)]
        public void RequiredOptionParsing_ValidInputs(string[] args, int expected)
        {
            var parser = ArgParserFactory.FromType<RequiredOptionsA>();
            var result = parser.Parse(args);

            Assert.Equal(expected, result.RequiredInt);

        }

        [Fact]
        public void MultipleRequiredOptionsParsing_ValidInputs()
        {
            var expected_r = 8;
            var expected_i = 5;

            string[] args = { "-r", expected_r.ToString(), "-i", expected_i.ToString()};

            var parser = ArgParserFactory.FromType<RequiredOptionsB>();
            var result = parser.Parse(args);

            var actual_r = result.RequiredIntA;
            var actual_i = result.RequiredIntB;

            Assert.Equal(expected_r, actual_r);
            Assert.Equal(expected_i, actual_i);
             

        }

    }
}
