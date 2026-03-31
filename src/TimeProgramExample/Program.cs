using ArgParser;
using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace TimeProgramExample;

[ExampleUsage("time [options] command [arguments...]")]
internal sealed class TimeArgs : BaseArgs
{
    [
        ShortNames('f'),
        LongNames("format"),
        Help(
            "Specify output format, possibly overriding the format specified in the environment variable TIME"
        ),
    ]
    public string? Format { get; set; }

    [ShortNames('p'), LongNames("portability"), Help("Use the portable output format")]
    public bool Portable { get; set; }

    [
        ShortNames('o'),
        LongNames("output"),
        Help("Do not send the results to stderr, but overwrite the specified file."),
        MetaVarName("FILE"),
    ]
    public string? Output { get; set; }

    [
        ShortNames('a'),
        LongNames("append"),
        Help("(Used together with -o.) Do not overwrite but append."),
        Requires(nameof(Output)),
    ]
    public bool Append { get; set; }

    [
        ShortNames('v'),
        LongNames("verbose"),
        Help("Give very verbose output about all the program knows about."),
    ]
    public bool Verbose { get; set; }

    [
        ShortNames('V'),
        LongNames("version"),
        Help("Print version information on standard output, then exit successfully."),
        TerminatingFlag<VersionCalledException>,
    ]
    public bool Version { get; set; }

    public override string[] PlainArguments { get; set; } = [];
}

internal class VersionCalledException : Exception;

internal static class Program
{
    internal static void Main(string[] args)
    {
        ArgParser<TimeArgs> argParser = ArgParserFactory.FromType<TimeArgs>();

        try
        {
            TimeArgs arguments = argParser.Parse(args);
            Run(arguments);
        }
        catch (CommandLineParsingException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (VersionCalledException)
        {
            Console.WriteLine("v1.0.1");
        }
        catch (HelpCalledException)
        {
            Console.WriteLine(argParser.GenerateHelpMessage());
        }
    }

    private static void RunProgram(string[] programWithArgs) { }

    private static void Run(TimeArgs args)
    {
        // Pretend the args are valid this is not here
        args = new TimeArgs
        {
            Format = "format",
            Portable = false,
            Output = "output",
            Append = false,
            Verbose = false,
        };

        if (args.Format is not null)
            Console.WriteLine(args.Format);

        RunProgram(args.PlainArguments);

        if (args.Portable)
            Console.WriteLine(
                """

                real 0.010s
                user 0.003s
                sys 0.008s
                """
            );
        else
            Console.WriteLine(
                """

                real    0m0.010s
                user    0m0.003s
                sys     0m0.008s
                """
            );
    }
}
