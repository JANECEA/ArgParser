using ArgParser;
using ArgParser.Attributes;

namespace Tests.ArgParserTests;

public class TerminatingFlagTests
{
    class TerminatingFlagExceptionA : Exception;

    class TerminatingFlagExceptionB : Exception;

    [ExampleUsage("program [options]")]
    internal sealed class TerminatingFlagsOptions : BaseArgs
    {
        public override string[] PlainArguments { get; set; } = [];

        [ShortNames('f'), TerminatingFlag<TerminatingFlagExceptionA>]
        public bool TerminatingFlagA { get; set; }

        [ShortNames('b'), LongNames("f", "longFlag"), TerminatingFlag<TerminatingFlagExceptionB>]
        public bool TerminatingFlagB { get; set; }

        [ShortNames('o'), LongNames("non_terminating_flag")]
        public bool NonTerminatingFlag { get; set; }

        [LongNames("integer")]
        public int? IntegerOption { get; set; }
    }

    [Theory]
    [InlineData(new[] { "-f" }, typeof(TerminatingFlagExceptionA))]
    [InlineData(new[] { "-b" }, typeof(TerminatingFlagExceptionB))]
    [InlineData(new[] { "--f" }, typeof(TerminatingFlagExceptionB))]
    [InlineData(new[] { "--longFlag" }, typeof(TerminatingFlagExceptionB))]
    [InlineData(new[] { "-f", "-b" }, typeof(TerminatingFlagExceptionA))]
    [InlineData(new[] { "-b", "-f" }, typeof(TerminatingFlagExceptionB))]
    [InlineData(new[] { "-o", "-f" }, typeof(TerminatingFlagExceptionA))]
    [InlineData(new[] { "--integer=9", "-f" }, typeof(TerminatingFlagExceptionA))]
    public void TerminatingFlags_ValidInput(string[] args, Type exceptionType)
    {
        var parser = ArgParserFactory.FromType<TerminatingFlagsOptions>();

        Assert.Throws(exceptionType, () => parser.Parse(args));
    }
}
