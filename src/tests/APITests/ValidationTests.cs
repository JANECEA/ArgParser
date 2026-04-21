using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.APITests;

public static class ValidationTests
{
    private class ValidationArgs : BaseArgs
    {
        [ShortNames('i')]
        [Range<int>(0, 50)]
        public int Age { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(49)]
    public static void CapturesInRange(int age)
    {
        var parser = ArgParserFactory.FromType<ValidationArgs>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs($"-i {age}"));

        Assert.Equal(age, result.Age);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(50)]
    [InlineData(123)]
    public static void ThrowsOnOutOfRange(int age)
    {
        var parser = ArgParserFactory.FromType<ValidationArgs>();

        Assert.Throws<ValidatorFailedException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs($"-i {age}"))
        );
    }

    private class InvalidValidationArgs : BaseArgs
    {
        [ShortNames('n')]
        [Range<int>(0, 50)]
        public string? Name { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void ThrowsOnNotEqualValidationTypes()
    {
        Assert.Throws<WrongValidatorTypeException>(
            ArgParserFactory.FromType<InvalidValidationArgs>
        );
    }

    public sealed class MustContainAttribute(string required) : OptionValidatorAttribute<string>
    {
        public override bool Validate(string arg, out string? errorMessage)
        {
            if (!arg.Contains(required))
            {
                errorMessage = $"The argument {arg} must contain {required}";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

    private class MustContainArgs : BaseArgs
    {
        [ShortNames('n')]
        [MustContain("@")]
        public string? Email { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("-n jenik")]
    [InlineData("-n still_nothing_here")]
    [InlineData("-n not_an_email_!#$$$&%")]
    public static void ThrowsOnUnsatisfiedCustomValidation(string args)
    {
        var parser = ArgParserFactory.FromType<MustContainArgs>();

        Assert.Throws<ValidatorFailedException>(() =>
            parser.Parse(ParsingHelper.GetSplitArgs(args))
        );
    }

    [Theory]
    [InlineData("-n j@s", "j@s")]
    [InlineData("-n @", "@")]
    [InlineData("-n jenik@seznam.cz", "jenik@seznam.cz")]
    public static void CapturesOnSatisfiedCustomValidation(string args, string expected)
    {
        var parser = ArgParserFactory.FromType<MustContainArgs>();

        var result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expected, result.Email);
    }
}
