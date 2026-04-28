using System.Diagnostics.CodeAnalysis;
using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace Tests.ArgParserTests;

public class CustomType_IntWrapper : IParsable<CustomType_IntWrapper>
{
    public CustomType_IntWrapper(int i) => WrappedInt = i;

    public int WrappedInt { get; init; }

    public static CustomType_IntWrapper Parse(string s, IFormatProvider? provider)
    {
        var success = TryParse(s, provider, out CustomType_IntWrapper? result);
        if (success)
            return result!;
        else
            throw new ArgumentException();
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out CustomType_IntWrapper result
    )
    {
        int wrapped = 0;
        var success = int.TryParse(s, provider, out wrapped);
        result = (success) ? new CustomType_IntWrapper(wrapped) : null;
        return success;
    }
}

public class CustomType_StringWrapper : IParsable<CustomType_StringWrapper>
{
    public string WrappedString { get; init; }

    public CustomType_StringWrapper(string s)
    {
        WrappedString = s;
    }

    public static CustomType_StringWrapper Parse(string s, IFormatProvider? provider)
    {
        return new CustomType_StringWrapper(s);
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out CustomType_StringWrapper result
    )
    {
        result = (s == null) ? null : new CustomType_StringWrapper(s);
        return s == null;
    }
}

public class BasicCustomOptionTypesTests
{
    [ExampleUsage("program [options]")]
    internal sealed class CustomOptions : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];

        [ShortNames('i'), LongNames("consumeInt")]
        public CustomType_IntWrapper? IntConsumer { get; set; }

        [ShortNames('s'), LongNames("consumeStr")]
        public CustomType_StringWrapper? StringConsumer { get; set; }

        [ShortNames('a'), LongNames("another")]
        public string? AnotherOption { get; set; }
    }

    public static IEnumerable<object?[]> CustomTypesValidInputs =>
        [
            [new[] { "-i", "5" }, new CustomType_IntWrapper(5), null],
            [new[] { "--consumeInt=5" }, new CustomType_IntWrapper(5), null],
            [new[] { "-s", "abc" }, null, new CustomType_StringWrapper("abc")],
            [
                new[] { "-i", "5", "-s", "abc" },
                new CustomType_IntWrapper(5),
                new CustomType_StringWrapper("abc"),
            ],
            [
                new[] { "-i", "5", "-s", "abc", "-a", "bcd" },
                new CustomType_IntWrapper(5),
                new CustomType_StringWrapper("abc"),
            ],
        ];

    [Theory]
    [MemberData(nameof(CustomTypesValidInputs))]
    public void CustomTypeParsing_ValidInputs(
        string[] args,
        CustomType_IntWrapper? expectedIntWrapper,
        CustomType_StringWrapper? expectedStringWrapper
    )
    {
        var parser = ArgParserFactory.FromType<CustomOptions>();
        var result = parser.Parse(args);

        if (expectedIntWrapper is null)
            Assert.Null(result.IntConsumer);
        else
            Assert.Equal(expectedIntWrapper.WrappedInt, result.IntConsumer!.WrappedInt);

        if (expectedStringWrapper is null)
            Assert.Null(result.StringConsumer);
        else
            Assert.Equal(expectedStringWrapper.WrappedString, result.StringConsumer!.WrappedString);
    }
}

public class CustomOptionTypesValidationTests
{
    internal sealed class MustBePositiveAttribute : OptionValidatorAttribute<CustomType_IntWrapper>
    {
        public override bool Validate(CustomType_IntWrapper arg, out string? errorMessage)
        {
            if (arg.WrappedInt <= 0)
            {
                errorMessage = $"Value {arg.WrappedInt} must be positive.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }

    internal sealed class MustBeEvenWrappedAttribute
        : OptionValidatorAttribute<CustomType_IntWrapper>
    {
        public override bool Validate(CustomType_IntWrapper arg, out string? errorMessage)
        {
            if (arg.WrappedInt % 2 != 0)
            {
                errorMessage = $"Value {arg.WrappedInt} must be even";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

    internal sealed class MustContainAtAttribute
        : OptionValidatorAttribute<CustomType_StringWrapper>
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

    internal sealed class MustBeLongerThanFiveAttribute
        : OptionValidatorAttribute<CustomType_StringWrapper>
    {
        public override bool Validate(CustomType_StringWrapper arg, out string? errorMessage)
        {
            if (arg.WrappedString.Length <= 5)
            {
                errorMessage = $"Value {arg.WrappedString} must be longer than 5 characters.";
                return false;
            }
            errorMessage = null;
            return true;
        }
    }

    [ExampleUsage("program [options]")]
    internal sealed class CustomOptionsValidators : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];

        [ShortNames('i'), LongNames("consumeInt"), MustBePositive, MustBeEvenWrapped]
        public CustomType_IntWrapper? IntConsumer { get; set; }

        [ShortNames('s'), LongNames("consumeStr"), MustContainAt, MustBeLongerThanFive]
        public CustomType_StringWrapper? StringConsumer { get; set; }
    }

    public static IEnumerable<object[]> ValidInputs =>
        [
            [new[] { "-i", "8" }],
            [new[] { "-s", "test@test.com" }],
            [new[] { "--consumeStr=hello@world" }],
            [new[] { "-i", "8", "-s", "test@test.com" }],
            [new[] { "-s", "hello@world", "-i", "8" }],
            [new[] { "--consumeInt=4", "--consumeStr=hello@world" }],
        ];

    [Theory]
    [MemberData(nameof(ValidInputs))]
    public void CustomTypeValidation_ValidInput(string[] args)
    {
        var parser = ArgParserFactory.FromType<CustomOptionsValidators>();
        var result = parser.Parse(args);
    }

    public static IEnumerable<object[]> InvalidInputs =>
        [
            [new[] { "-i", "-2" }],
            [new[] { "-i", "0" }],
            [new[] { "-i", "3" }],
            [new[] { "-s", "a@b" }],
            [new[] { "-s", "veryveryveryverylong" }],
            [new[] { "-i", "3", "-s", "test@test.com" }],
            [new[] { "-i", "2", "-s", "noemail" }],
            [new[] { "--consumeInt=3", "--consumeStr=x@y" }],
        ];

    [Theory]
    [MemberData(nameof(InvalidInputs))]
    public void CustomTypeValidation_InvalidInput(string[] args)
    {
        var parser = ArgParserFactory.FromType<CustomOptionsValidators>();

        Assert.Throws<ValidatorFailedException>(() => parser.Parse(args));
    }
}
