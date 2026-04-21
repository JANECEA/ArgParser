using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.APITests;

public static class PlainArguments
{
    public static class AllowedPlainArguments
    {
        [AllowPlainArguments(true)]
        private class EmptyArgsWithCapture : BaseArgs
        {
            public override string[] PlainArguments { get; set; } = [];
        }

        [Theory]
        [InlineData("", new string[] { })]
        [InlineData("one", new[] { "one" })]
        [InlineData("a b c", new[] { "a", "b", "c" })]
        public static void CapturedWithoutDelimiter(string args, string[] expected)
        {
            var parser = ArgParserFactory.FromType<EmptyArgsWithCapture>();

            var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

            Assert.Equal(expected, result.PlainArguments);
        }

        [Theory]
        [InlineData("--", new string[] { })]
        [InlineData("-- one", new[] { "one" })]
        [InlineData("-- one two three", new[] { "one", "two", "three" })]
        [InlineData("-- --one two", new[] { "--one", "two" })]
        [InlineData("-- -o two", new[] { "-o", "two" })]
        [InlineData("first -- -o two", new[] { "first", "-o", "two" })]
        [InlineData("first second --", new[] { "first", "second" })]
        public static void CapturedWithDelimiter(string args, string[] expected)
        {
            var parser = ArgParserFactory.FromType<EmptyArgsWithCapture>();

            var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

            Assert.Equal(expected, result.PlainArguments);
        }

        [AllowPlainArguments(true)]
        private class PlainArgsRequiredArgs : BaseArgs
        {
            [Required]
            public override string[] PlainArguments { get; set; } = [];
        }

        [Theory]
        [InlineData("", new string[] { })]
        [InlineData("--", new string[] { })]
        public static void ThrowsWhenEmpty(string args, string[] expected)
        {
            var parser = ArgParserFactory.FromType<PlainArgsRequiredArgs>();

            var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

            Assert.Equal(expected, result.PlainArguments);
        }
    }

    public static class NotAllowedPlainArguments
    {
        [AllowPlainArguments(false)]
        private class EmptyArgsWithoutCapture : BaseArgs
        {
            public override string[] PlainArguments { get; set; } = [];
        }

        [Theory]
        [InlineData("", new string[] { })]
        [InlineData("--", new string[] { })]
        public static void EmptyPlainArgsNotThrowing(string args, string[] expected)
        {
            var parser = ArgParserFactory.FromType<EmptyArgsWithoutCapture>();

            var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

            Assert.Equal(expected, result.PlainArguments);
        }

        [Theory]
        [InlineData("one")]
        [InlineData("a b c")]
        [InlineData("-- one two three")]
        [InlineData("first -- -o two")]
        public static void NonemptyPlainArgsThrowing(string args)
        {
            var parser = ArgParserFactory.FromType<EmptyArgsWithoutCapture>();

            // I could not really find inside the docs what exception should be thrown here, so keep it abstract...
            Assert.Throws<CommandLineParsingException>(() =>
                parser.Parse(ParsingHelper.GetSplitArgs(args))
            );
        }

        [AllowPlainArguments(false)]
        private class PlainArgsRequiredButForbiddenArgs : BaseArgs
        {
            [Required]
            public override string[] PlainArguments { get; set; } = [];
        }

        [Fact]
        public static void RequiredPlainArgsThrowingDuringCreation()
        {
            Assert.ThrowsAny<ParserConfigurationException>(
                ArgParserFactory.FromType<PlainArgsRequiredButForbiddenArgs>
            );
        }
    }
}
