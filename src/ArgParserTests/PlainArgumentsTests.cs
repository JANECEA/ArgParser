using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace ArgParserTests
{
    public class PlainArgumentsTests
    {
        [AllowPlainArguments(true), ExampleUsage("myProgram [options]")]
        public class PlainArgumentsCatcher : BaseArgs
        {
            [ShortNames('i')]
            public int? IntegerOption { get; set; }

            [ShortNames('s')]
            public string? StringOption { get; set; }

            [ShortNames('b')]
            public bool Flag { get; set; }

            public override string[] PlainArguments { get; set; } = [];
        }

        public static IEnumerable<object[]> PlainArgumentsValidInputs =>
            [
                [new[] { "--", "plain1" }, new[] { "plain1" }],
                [new[] { "--", "plain1", "plain2" }, new[] { "plain1", "plain2" }],
                [new[] { "-i", "8", "--", "plain1" }, new[] { "plain1" }],
                [new[] { "-b", "--", "plain1", "plain2" }, new[] { "plain1", "plain2" }],
                [
                    new[] { "-i", "8", "-s", "abc", "--", "plain1", "plain2" },
                    new[] { "plain1", "plain2" },
                ],
                [new[] { "--", "-i", "8" }, new[] { "-i", "8" }],
                [new[] { "--", "--nonexistent" }, new[] { "--nonexistent" }],
            ];

        [Theory]
        [MemberData(nameof(PlainArgumentsValidInputs))]
        public void PlainArgumentsWithSeparator_ValidInput(string[] args, string[] expectedPlain)
        {
            var parser = ArgParserFactory.FromType<PlainArgumentsCatcher>();
            var result = parser.Parse(args);

            Assert.Equal(expectedPlain, result.PlainArguments);
        }

        public static IEnumerable<object[]> PlainArgumentsInvalidInputs =>
            [
                [new[] { "plain1" }],
                [new[] { "-i", "8", "plain1" }],
                [new[] { "-b", "plain1" }],
                [new[] { "--nonexistent", "--", "plain1" }],
            ];

        [Theory]
        [MemberData(nameof(PlainArgumentsInvalidInputs))]
        public void PlainArgumentsWithSeparator_InvalidInput(string[] args)
        {
            var parser = ArgParserFactory.FromType<PlainArgumentsCatcher>();

            Assert.Throws<CommandLineParsingException>(() => parser.Parse(args));
        }
    }
}
