using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace APITests;

public static class TerminatingFlagTests
{
    private class QuitException : Exception;
    
    private sealed class TerminatingArgs : BaseArgs
    {
        [ShortNames('q'), LongNames("quit"), TerminatingFlag<QuitException>]
        public bool Quit { get; set; }
        
        [ShortNames('i')]
        public int IntVal { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("-q")]
    [InlineData("--quit")]
    [InlineData("--quit -i 123")]
    [InlineData("-q -i 123")]
    [InlineData("-i 123 --quit")]
    [InlineData("-i 123 -q")]
    public static void TerminatingFlagSpecifiedThrowsConfiguredException(string args)
    {
        var parser = ArgParserFactory.FromType<TerminatingArgs>();

        // Note that the API or docs does not specify that the Parse method should throw exception
        // of such type, but it seems to be the case.
        Assert.Throws<QuitException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }

    private sealed class HelpLikeArgs : BaseArgs
    {
        [ShortNames('h'), LongNames("help"), TerminatingFlag<HelpCalledException>]
        public bool Help { get; set; }

        public override string[] PlainArguments { get; set; } = [];
    }

    [Theory]
    [InlineData("-h")]
    [InlineData("--help")]
    public static void HelpTerminatingFlagThrowsHelpCalledException(string args)
    {
        var parser = ArgParserFactory.FromType<HelpLikeArgs>();

        Assert.Throws<HelpCalledException>(() => parser.Parse(ParsingHelper.GetSplitArgs(args)));
    }
    
    private class ExitException: Exception;
    
    private sealed class DoubleTerminatingArgs : BaseArgs
    {
        [ShortNames('q'), LongNames("quit"), TerminatingFlag<QuitException>]
        public bool Quit { get; set; }
        
        [ShortNames('e'), LongNames("exit"), TerminatingFlag<ExitException>]
        public bool Exit { get; set; }
        
        public override string[] PlainArguments { get; set; } = [];
    }
    
    [Fact]
    public static void OrderOfTerminatingFlagsMatters1()
    {
        var parser = ArgParserFactory.FromType<DoubleTerminatingArgs>();
        
        Assert.Throws<QuitException>(() => parser.Parse(ParsingHelper.GetSplitArgs("-q -e")));
    }
    
    [Fact]
    public static void OrderOfTerminatingFlagsMatters2()
    {
        var parser = ArgParserFactory.FromType<DoubleTerminatingArgs>();
        Assert.Throws<ExitException>(() => parser.Parse(ParsingHelper.GetSplitArgs("-e -q")));
    }
    
    class TerminatingNotOnFlagArgs : BaseArgs
    {
        [ShortNames('o')]
        [TerminatingFlag<Exception>]
        public string? Output { get; set; }
        
        public override string[] PlainArguments { get; set; } = [];
    }
    
    [Fact]
    public static void TerminatingNotOnFlagThrows()
    {
        Assert.Throws<TerminatingNotOnFlagException>(ArgParserFactory.FromType<TerminatingNotOnFlagArgs>);
    }
}