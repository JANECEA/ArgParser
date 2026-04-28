using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.ArgParserTests;

public class ExclusiveTests
{
    internal sealed class ExclusiveOptionAAndBAttribute : ClassValidatorAttribute<ExclusiveOptions>
    {
        public override bool Validate(ExclusiveOptions args, out string? errorMessage)
        {
            if (args.OptionA is not null && args.OptionB is not null)
            {
                errorMessage = "OptionA and OptionB must not be set at the same time.";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

    internal sealed class ExclusiveOptionBAndCAttribute : ClassValidatorAttribute<ExclusiveOptions>
    {
        public override bool Validate(ExclusiveOptions args, out string? errorMessage)
        {
            if (args.OptionB is not null && args.OptionC is not null)
            {
                errorMessage = "OptionB and OptionC must not be set at the same time.";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

    internal sealed class ExclusiveOptionBAndDAttribute : ClassValidatorAttribute<ExclusiveOptions>
    {
        public override bool Validate(ExclusiveOptions args, out string? errorMessage)
        {
            if (args.OptionD is not null && args.OptionB is not null)
            {
                errorMessage = "OptionB and OptionD must not be set at the same time.";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

    internal sealed class ExclusiveOptionCAndDAttribute : ClassValidatorAttribute<ExclusiveOptions>
    {
        public override bool Validate(ExclusiveOptions args, out string? errorMessage)
        {
            if (args.OptionC is not null && args.OptionD is not null)
            {
                errorMessage = "OptionC and OptionD must not be set at the same time.";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

    [
        ExclusiveOptionAAndB,
        ExclusiveOptionBAndC,
        ExclusiveOptionBAndD,
        ExclusiveOptionCAndD,
        ExampleUsage("program [options]")
    ]
    internal sealed class ExclusiveOptions : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];

        // A exclusive with B
        // B exclusive with C and D
        // C exclusive with D

        [ShortNames('a')]
        public int? OptionA { get; set; }

        [ShortNames('b')]
        public int? OptionB { get; set; }

        [ShortNames('c')]
        public int? OptionC { get; set; }

        [ShortNames('d')]
        public int? OptionD { get; set; }

        [ShortNames('e')]
        public int? OptionE { get; set; }
    }

    public static IEnumerable<object[]> ValidInputs =>
        [
            [new[] { "-a", "8" }],
            [new[] { "-a", "8", "-d", "4" }],
            [new[] { "-a", "8", "-c", "4" }],
            [new[] { "-b", "8", "-e", "4" }],
        ];

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void MutualyExclusive_ValidInput(string[] args)
    {
        var parser = ArgParserFactory.FromType<ExclusiveOptions>();
        var result = parser.Parse(args);
    }

    public static IEnumerable<object[]> InvalidInputs =>
        [
            [new[] { "-a", "8", "-b", "4" }],
            [new[] { "-d", "8", "-c", "4" }],
            [new[] { "-b", "8", "-c", "4", "-e", "4" }],
            [new[] { "-d", "8", "-b", "4" }],
        ];

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public void MutualyExclusive_InvalidInput(string[] args)
    {
        var parser = ArgParserFactory.FromType<ExclusiveOptions>();
        Assert.Throws<ValidatorFailedException>(() => parser.Parse(args));
    }
}
