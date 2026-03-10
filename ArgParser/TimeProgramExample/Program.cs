using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace TimeProgramExample;

[ExampleUsage("time [options] command [arguments...]")]
internal sealed class TimeArgs : BaseArgs
{
    [
        ShortOptions('f'),
        LongOptions("format"),
        Help(
            "Specify output format, possibly overriding the format specified in the environment variable TIME"
        ),
    ]
    public string? Format { get; set; }

    [ShortOptions('p'), LongOptions("portability"), Help("Use the portable output format")]
    public bool Portability { get; set; }

    [
        ShortOptions('o'),
        LongOptions("output"),
        Help("Do not send the results to stderr, but overwrite the specified file."),
        ValuePlaceholder("FILE"),
    ]
    public string? Output { get; set; }

    [
        ShortOptions('a'),
        LongOptions("append"),
        Help("(Used together with -o.) Do not overwrite but append."),
        Requires(nameof(Output)),
    ]
    public bool Append { get; set; }

    [
        ShortOptions('v'),
        LongOptions("verbose"),
        Help("Give very verbose output about all the program knows about."),
    ]
    public bool Verbose { get; set; }

    [
        ShortOptions('V'),
        LongOptions("version"),
        Help("Print version information on standard output, then exit successfully."),
    ]
    public bool Version { get; set; }

    public override string[] PlainArguments { get; set; }
}

internal static class Program
{
    internal static void Main(string[] args)
    {
        ArgParser<TimeArgs> argParser = ArgParserFactory.FromType<TimeArgs>();

        try
        {
            TimeArgs arguments = argParser.Parse(args);
        }
        catch (CommandLineParsingException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (HelpCalledException helpEx)
        {
            Console.WriteLine(helpEx.HelpMessage);
        }
    }
}
