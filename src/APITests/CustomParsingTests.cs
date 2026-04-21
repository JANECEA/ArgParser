using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace APITests;

public static class CustomParsingTests
{
    private record Person(string Name, string Surname, int Age);

    private class PersonUnparsableArgs : BaseArgs
    {
        public Person? Person { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void ThrowsWhenUnparsable()
    {
        Assert.Throws<PropertyNotParsableException>(
            ArgParserFactory.FromType<PersonUnparsableArgs>
        );
    }

    private record ParsablePerson(string Name, string Surname, int Age)
        : Person(Name, Surname, Age),
            IParsable<ParsablePerson>
    {
        public static ParsablePerson Parse(string s, IFormatProvider? provider)
        {
            var parts = s.Split(';');
            return new(parts[0], parts[1], int.Parse(parts[2]));
        }

        public static bool TryParse(
            [NotNullWhen(true)] string? s,
            IFormatProvider? provider,
            [MaybeNullWhen(false)] out ParsablePerson result
        )
        {
            var parts = s!.Split(';');
            result = new(parts[0], parts[1], int.Parse(parts[2]));
            return true;
        }
    }

    private class PersonParsableArgs : BaseArgs
    {
        [LongNames("person")]
        public ParsablePerson? ParsablePerson { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Fact]
    public static void ParsesIParsableCustomType()
    {
        var parser = ArgParserFactory.FromType<PersonParsableArgs>();
        var args = parser.Parse(ParsingHelper.GetSplitArgs("--person Jenik;Pernik;30"));
        Assert.Equal(new ParsablePerson("Jenik", "Pernik", 30), args.ParsablePerson);
    }
}
