using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.PositionalArgsTests;

public static class PositionalArgsConfigurationTests
{
    [PositionalArgs(nameof(Input), nameof(Output))]
    private class ValidPositionalConfigArgs : BaseArgs
    {
        public string? Input { get; set; }

        public string? Output { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void ValidPositionalConfigurationDoesNotThrow()
    {
        var parser = ArgParserFactory.FromType<ValidPositionalConfigArgs>();

        Assert.NotNull(parser);
    }

    [PositionalArgs("DoesNotExist")]
    private class MissingPositionalPropertyArgs : BaseArgs
    {
        public string? Existing { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(Existing), nameof(Existing))]
    private class DuplicatePositionalPropertyArgs : BaseArgs
    {
        public string? Existing { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(Name))]
    private class PositionalWithShortNameArgs : BaseArgs
    {
        [ShortNames('n')]
        public string? Name { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(Name))]
    private class PositionalWithLongNameArgs : BaseArgs
    {
        [LongNames("name")]
        public string? Name { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(Stop))]
    private class PositionalWithTerminatingFlagArgs : BaseArgs
    {
        [TerminatingFlag<InvalidOperationException>]
        public bool Stop { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(Number))]
    private class PositionalWithInvalidValidatorTypeArgs : BaseArgs
    {
        [Range<int>(0, 10)]
        public string? Number { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(Value))]
    private class PositionalWithUnparsableTypeArgs : BaseArgs
    {
        public object? Value { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(First), "Missing", nameof(Second))]
    private class PositionalWithMissingInMiddleArgs : BaseArgs
    {
        public string? First { get; set; }

        public string? Second { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(One), nameof(Two), nameof(One))]
    private class PositionalDuplicateLaterArgs : BaseArgs
    {
        public string? One { get; set; }

        public string? Two { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [PositionalArgs(nameof(Target))]
    private class PositionalWithAllConflictingAttributesArgs : BaseArgs
    {
        [ShortNames('t')]
        [LongNames("target")]
        [TerminatingFlag<InvalidOperationException>]
        public bool Target { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    public static TheoryData<Func<object>, Type> InvalidConfigurations =>
        new()
        {
            {
                ArgParserFactory.FromType<MissingPositionalPropertyArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<DuplicatePositionalPropertyArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<PositionalWithShortNameArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<PositionalWithLongNameArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<PositionalWithTerminatingFlagArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<PositionalWithMissingInMiddleArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<PositionalDuplicateLaterArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<PositionalWithAllConflictingAttributesArgs>,
                typeof(PositionalArgsConfigException)
            },
            {
                ArgParserFactory.FromType<PositionalWithInvalidValidatorTypeArgs>,
                typeof(WrongValidatorTypeException)
            },
            {
                ArgParserFactory.FromType<PositionalWithUnparsableTypeArgs>,
                typeof(PropertyNotParsableException)
            },
        };

    [Theory]
    [MemberData(nameof(InvalidConfigurations))]
    public static void ThrowsOnInvalidPositionalConfiguration(
        Func<object> parserFactory,
        Type expectedExceptionType
    )
    {
        Assert.Throws(expectedExceptionType, parserFactory);
    }
}
