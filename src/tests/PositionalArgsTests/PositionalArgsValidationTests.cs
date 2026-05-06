using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;
using Tests.APITests;

namespace Tests.PositionalArgsTests;

public static class PositionalArgsValidationTests
{
    [PositionalArgs(nameof(Count), nameof(Name), nameof(Channel))]
    private class RequiredPositionalArgs : BaseArgs
    {
        [Required]
        public int Count { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Channel { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData(""),
        InlineData("1 two"),
        InlineData("--"),
        InlineData("1 --"),
        InlineData("1 -- two"),
        InlineData("1 two --"),
        InlineData("15"),
        InlineData("15 alpha"),
        InlineData("-- 15"),
        InlineData("-- 15 alpha"),
    ]
    public static void ThrowsWhenRequiredPositionalArgumentsAreMissing(string args)
    {
        var parser = ArgParserFactory.FromType<RequiredPositionalArgs>();

        Assert.Throws<MissingRequiredOptionException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [PositionalArgs(nameof(Amount), nameof(User))]
    private class TypedPositionalArgs : BaseArgs
    {
        public int Amount { get; set; }

        public string? User { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData("x alice"),
        InlineData("abc bob"),
        InlineData("one clara"),
        InlineData("-- x dana"),
        InlineData("-- abc eva"),
        InlineData("value frank"),
        InlineData("nope george"),
        InlineData("-- nope helen"),
        InlineData("z1 isaac"),
        InlineData("-- z2 jack"),
    ]
    public static void ThrowsOnInvalidTypedPositionalValue(string args)
    {
        var parser = ArgParserFactory.FromType<TypedPositionalArgs>();

        Assert.Throws<ValueParsingException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }

    [PositionalArgs(nameof(Amount), nameof(User))]
    private class PositionalRangeValidationArgs : BaseArgs
    {
        [Range<int>(1, 11)]
        public int Amount { get; set; }

        public string? User { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData("1 alice", 1, "alice"),
        InlineData("2 bob", 2, "bob"),
        InlineData("3 clara", 3, "clara"),
        InlineData("4 dana", 4, "dana"),
        InlineData("5 eva", 5, "eva"),
        InlineData("6 frank", 6, "frank"),
        InlineData("7 george", 7, "george"),
        InlineData("8 helen", 8, "helen"),
        InlineData("9 isaac", 9, "isaac"),
        InlineData("10 jack", 10, "jack"),
        InlineData("-- 4 kate", 4, "kate"),
        InlineData("-- 10 luke", 10, "luke"),
    ]
    public static void AppliesValidatorsOnPositionalValues(
        string args,
        int expectedAmount,
        string expectedUser
    )
    {
        var parser = ArgParserFactory.FromType<PositionalRangeValidationArgs>();

        PositionalRangeValidationArgs result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expectedAmount, result.Amount);
        Assert.Equal(expectedUser, result.User);
    }

    [
        Theory,
        InlineData("0 alice"),
        InlineData("11 bob"),
        InlineData("12 clara"),
        InlineData("50 dana"),
        InlineData("-- 0 eva"),
        InlineData("-- 11 frank"),
        InlineData("-- 20 george"),
        InlineData("-- 100 helen"),
        InlineData("999 isaac"),
        InlineData("-- 999 jack"),
    ]
    public static void ThrowsWhenPositionalValidatorFails(string args)
    {
        var parser = ArgParserFactory.FromType<PositionalRangeValidationArgs>();

        Assert.Throws<ValidatorFailedException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [PositionalArgs(nameof(User), nameof(Token))]
    private class PositionalRequiresArgs : BaseArgs
    {
        [Requires(nameof(Token))]
        public string? User { get; set; }

        public string? Token { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData("alice"),
        InlineData("-- alice"),
        InlineData("alice --"),
    ]
    public static void ThrowsWhenPositionalRequiresDependencyIsMissing(string args)
    {
        var parser = ArgParserFactory.FromType<PositionalRequiresArgs>();

        Assert.Throws<MissingRequiredOptionException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [
        Theory,
        InlineData("alice token", "alice", "token"),
        InlineData("bob t2", "bob", "t2"),
        InlineData("-- clara t3", "clara", "t3"),
        InlineData("dana -- t4", "dana", "t4"),
        InlineData("-- eva t5", "eva", "t5"),
        InlineData("frank t6 extra", "frank", "t6"),
        InlineData("george t7 -- trailing", "george", "t7"),
        InlineData("-- helen t8 -- trailing", "helen", "t8"),
    ]
    public static void SatisfiesRequiresWhenDependentPositionalIsPresent(
        string args,
        string expectedUser,
        string expectedToken
    )
    {
        var parser = ArgParserFactory.FromType<PositionalRequiresArgs>();

        PositionalRequiresArgs result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expectedUser, result.User);
        Assert.Equal(expectedToken, result.Token);
    }
}
