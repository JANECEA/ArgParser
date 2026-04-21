using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;


namespace ArgParserTests
{
    public class ValidationTests
    {
        internal sealed class MustBeEvenAttribute : OptionValidatorAttribute<int>
        {
            public override bool Validate(int arg, out string? errorMessage)
            {
                if (arg % 2 != 0)
                {
                    errorMessage = $"The argument {arg} must be even.";
                    return false;
                }
                errorMessage = null;
                return true;
            }
        }
        internal sealed class MustContainAtAttribute : OptionValidatorAttribute<CustomType_StringWrapper>
        {
            public override bool Validate(CustomType_StringWrapper arg, out string? errorMessage)
            {
                if (!arg.WrappedString.Contains('@'))
                {
                    errorMessage = $"Value {arg.WrappedString} must contain @.";
                    return false;

                }
                errorMessage = null;
                return true;
            }
        }

        [ExampleUsage("program [options]")]
        internal sealed class SimpleArgs_WithValidators : BaseArgs
        {
            public override string[] PlainArguments { get; set; } = [];

            [ShortNames('i'), LongNames("int"), Range<int>(0, 100)]
            public int? IntegerOption { get; set; }

            [ShortNames('e'), LongNames("even"), MustBeEven]
            public int? EvenOption { get; set; }

            [ShortNames('s'), LongNames("string"), MustContainAt]
            public string? StringOption { get; set; }
        }

        public static IEnumerable<object[]> ValidatorValidInputs =>
            [
                [new[] { "-i", "0"}],
                [new[] { "-i", "50" }],
                [new[] { "-i", "100" }],
                [new[] { "-e", "0"}],
                [new[] { "-e", "2" }],
                [new[] {"-e", "-4" }],
                [new[] { "-e", "1000"}],
                [new[] { "-s", "a@b" }],
                [new[] { "-s", "test@test.com" }],
                [new[] {"-i", "8", "-s", "@ooo", "-e","4268"}]
            ];

        [Theory]
        [MemberData(nameof(ValidatorValidInputs))]
        public void PrimitiveTypesValidation_ValidInputs(string[] args)
        {
            var parser = ArgParserFactory.FromType<SimpleArgs_WithValidators>();
            var result = parser.Parse(args);
        }

        public static IEnumerable<object[]> ValidatorInvalidInputs =>
            [
                [new[] { "-i", "-1" }],
                [new[] { "-i", "101" }],
                [new[] { "-i", "1000" }],
                [new[] { "-e", "1" }],
                [new[] { "-e", "3" }],
                [new[] { "-e", "-7" }],
                [new[] { "-s", "noemail" }],
                [new[] { "-s", "missingatsign" }],
                [new[] {"-s", "g@g", "-i","102"}],
                [new[] {"-s","stodva","-i","102", "-e","101"}]
            ];

        [Theory]
        [MemberData(nameof(ValidatorInvalidInputs))]
        public void PrimitiveTypesValidation_InvalidInputs(string[] args)
        {
            var parser = ArgParserFactory.FromType<SimpleArgs_WithValidators>();
            Assert.Throws<CommandLineParsingException>(() => parser.Parse(args));
        }

    }
}
