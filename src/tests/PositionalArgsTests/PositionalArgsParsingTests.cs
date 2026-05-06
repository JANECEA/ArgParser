using ArgParser;
using ArgParser.Attributes;
using Tests.APITests;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Tests.PositionalArgsTests;

public static class PositionalArgsParsingTests
{
    [PositionalArgs(nameof(Third), nameof(First), nameof(Second))]
    private class OrderedTripletArgs : BaseArgs
    {
        public string? First { get; set; }

        public int Second { get; set; }

        public string? Third { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData("seed alpha 1", "seed", "alpha", 1),
        InlineData("begin beta 2", "begin", "beta", 2),
        InlineData("src gamma 3", "src", "gamma", 3),
        InlineData("left delta 4", "left", "delta", 4),
        InlineData("up epsilon 5", "up", "epsilon", 5),
        InlineData("down zeta 6", "down", "zeta", 6),
        InlineData("north eta 7", "north", "eta", 7),
        InlineData("south theta 8", "south", "theta", 8),
        InlineData("east iota 9", "east", "iota", 9),
        InlineData("west kappa 10", "west", "kappa", 10),
        InlineData("foo lambda 11", "foo", "lambda", 11),
        InlineData("bar mu 12", "bar", "mu", 12),
        InlineData("item nu 13", "item", "nu", 13),
        InlineData("doc xi 14", "doc", "xi", 14),
        InlineData("task omicron 15", "task", "omicron", 15),
    ]
    public static void MapsPositionalValuesByAttributeOrder(
        string args,
        string expectedThird,
        string expectedFirst,
        int expectedSecond
    )
    {
        var parser = ArgParserFactory.FromType<OrderedTripletArgs>();

        OrderedTripletArgs result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expectedThird, result.Third);
        Assert.Equal(expectedFirst, result.First);
        Assert.Equal(expectedSecond, result.Second);
        Assert.Empty(result.PlainArguments);
    }

    [PositionalArgs(nameof(Input), nameof(Output), nameof(Profile))]
    private class PositionalWithNamedArgs : BaseArgs
    {
        public string? Input { get; set; }

        public string? Output { get; set; }

        public string? Profile { get; set; }

        [ShortNames('v')]
        public bool Verbose { get; set; }

        [LongNames("count")]
        public int? Count { get; set; }

        [LongNames("label")]
        public string? Label { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData("in out prod", false, null, null, "in", "out", "prod"),
        InlineData("-v in out dev", true, null, null, "in", "out", "dev"),
        InlineData("in -v out qa", true, null, null, "in", "out", "qa"),
        InlineData("--count 1 in out perf", false, 1, null, "in", "out", "perf"),
        InlineData("--label nightly in out canary", false, null, "nightly", "in", "out", "canary"),
        InlineData("--count 2 --label dry in out local", false, 2, "dry", "in", "out", "local"),
        InlineData("-v --count 3 in out release", true, 3, null, "in", "out", "release"),
        InlineData("--count 4 in -v out stage", true, 4, null, "in", "out", "stage"),
        InlineData("in --count 5 out test", false, 5, null, "in", "out", "test"),
        InlineData("in out --count 6 hotfix", false, 6, null, "in", "out", "hotfix"),
        InlineData("in out prod --count 7", false, 7, null, "in", "out", "prod"),
        InlineData("--label fast in -v out blue", true, null, "fast", "in", "out", "blue"),
        InlineData("-v --label green in out gray", true, null, "green", "in", "out", "gray"),
        InlineData("--count 8 in out --label tagged demo", false, 8, "tagged", "in", "out", "demo"),
        InlineData("--count 9 --label l in out x", false, 9, "l", "in", "out", "x"),
        InlineData("in --label adHoc out y", false, null, "adHoc", "in", "out", "y"),
        InlineData("-v in out --label batch z", true, null, "batch", "in", "out", "z"),
        InlineData("--label one --count 10 in out two", false, 10, "one", "in", "out", "two"),
    ]
    public static void ParsesPositionalAndNamedArgumentsTogether(
        string args,
        bool expectedVerbose,
        int? expectedCount,
        string? expectedLabel,
        string expectedInput,
        string expectedOutput,
        string expectedProfile
    )
    {
        var parser = ArgParserFactory.FromType<PositionalWithNamedArgs>();

        PositionalWithNamedArgs result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expectedVerbose, result.Verbose);
        Assert.Equal(expectedCount, result.Count);
        Assert.Equal(expectedLabel, result.Label);
        Assert.Equal(expectedInput, result.Input);
        Assert.Equal(expectedOutput, result.Output);
        Assert.Equal(expectedProfile, result.Profile);
        Assert.Empty(result.PlainArguments);
    }

    [PositionalArgs(nameof(User), nameof(Project), nameof(Stage))]
    private class DelimiterAndRemainderArgs : BaseArgs
    {
        public string? User { get; set; }

        public string? Project { get; set; }

        public string? Stage { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData("jane api dev tail", "jane", "api", "dev", new[] { "tail" }),
        InlineData("jane api dev one two", "jane", "api", "dev", new[] { "one", "two" }),
        InlineData("jane api -- dev one", "jane", "api", "dev", new[] { "one" }),
        InlineData("jane -- api dev one", "jane", "api", "dev", new[] { "one" }),
        InlineData("-- jane api dev one", "jane", "api", "dev", new[] { "one" }),
        InlineData("jane api dev -- one", "jane", "api", "dev", new[] { "one" }),
        InlineData("jane api -- --literal one", "jane", "api", "--literal", new[] { "one" }),
        InlineData("jane -- api --literal one", "jane", "api", "--literal", new[] { "one" }),
        InlineData("-- jane -- api dev one", "jane", "--", "api", new[] { "dev", "one" }),
        InlineData("a b c d e f", "a", "b", "c", new[] { "d", "e", "f" }),
        InlineData("a b -- c d e", "a", "b", "c", new[] { "d", "e" }),
        InlineData("a -- b c d e", "a", "b", "c", new[] { "d", "e" }),
        InlineData("-- a b c d e", "a", "b", "c", new[] { "d", "e" }),
        InlineData("-- a b -- c d", "a", "b", "--", new[] { "c", "d" }),
        InlineData(
            "alpha beta gamma -- --tail x",
            "alpha",
            "beta",
            "gamma",
            new[] { "--tail", "x" }
        ),
    ]
    public static void ConsumesPositionalValuesAcrossDelimiterAndLeavesRemainder(
        string args,
        string expectedUser,
        string expectedProject,
        string expectedStage,
        string[] expectedPlain
    )
    {
        var parser = ArgParserFactory.FromType<DelimiterAndRemainderArgs>();

        DelimiterAndRemainderArgs result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expectedUser, result.User);
        Assert.Equal(expectedProject, result.Project);
        Assert.Equal(expectedStage, result.Stage);
        Assert.Equal(expectedPlain, result.PlainArguments);
    }

    [PositionalArgs(nameof(A), nameof(B), nameof(C), nameof(D))]
    private class FourPositionalsArgs : BaseArgs
    {
        public string? A { get; set; }

        public string? B { get; set; }

        public string? C { get; set; }

        public string? D { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [
        Theory,
        InlineData("a b c d", "a", "b", "c", "d", new string[] { }),
        InlineData("a b c d e", "a", "b", "c", "d", new[] { "e" }),
        InlineData("a b c d e f", "a", "b", "c", "d", new[] { "e", "f" }),
        InlineData("-- a b c d e", "a", "b", "c", "d", new[] { "e" }),
        InlineData("a -- b c d e", "a", "b", "c", "d", new[] { "e" }),
        InlineData("a b -- c d e", "a", "b", "c", "d", new[] { "e" }),
        InlineData("a b c -- d e", "a", "b", "c", "d", new[] { "e" }),
        InlineData("a b c d -- e", "a", "b", "c", "d", new[] { "e" }),
        InlineData("-- a -- b -- c d e", "a", "--", "b", "--", new[] { "c", "d", "e" }),
        InlineData(
            "one two three four five six seven",
            "one",
            "two",
            "three",
            "four",
            new[] { "five", "six", "seven" }
        ),
    ]
    public static void MapsMultiplePositionalsAndPreservesUnmapped(
        string args,
        string expectedA,
        string expectedB,
        string expectedC,
        string expectedD,
        string[] expectedPlain
    )
    {
        var parser = ArgParserFactory.FromType<FourPositionalsArgs>();

        FourPositionalsArgs result = parser.Parse(ParsingHelper.GetSplitArgs(args));

        Assert.Equal(expectedA, result.A);
        Assert.Equal(expectedB, result.B);
        Assert.Equal(expectedC, result.C);
        Assert.Equal(expectedD, result.D);
        Assert.Equal(expectedPlain, result.PlainArguments);
    }
}
