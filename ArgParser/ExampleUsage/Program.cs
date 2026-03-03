using ArgParser;
using ArgParser.Attributes;

namespace ExampleUsage;

internal sealed class Args : BaseArgs
{
    [
        ShortOptions("-f"),
        LongOptions("--format"),
        Help(
            "Specify output format, possibly overriding the format specified in the environment variable TIME"
        ),
        Required,
        Range<string>("Aa", "bb"),
    ]
    public string? Format { get; set; }

    [ShortOptions("-p"), LongOptions("--portability"), Help("Use the portable output format")]
    public bool Portability { get; set; }

    [
        ShortOptions("-o"),
        LongOptions("--output"),
        Help("Do not send the results to stderr, but overwrite the specified file."),
        ExistsValidator,
    ]
    public string? Output { get; set; }

    [
        ShortOptions("-a"),
        LongOptions("--append"),
        Help("(Used together with -o.) Do not overwrite but append."),
        Requires(nameof(Output)),
    ]
    public bool Append { get; set; }

    [
        ShortOptions("-v"),
        LongOptions("--verbose"),
        Help("Give very verbose output about all the program knows about."),
    ]
    public bool Verbose { get; set; }

    [
        ShortOptions("-V"),
        LongOptions("--version"),
        Help("Print version information on standard output, then exit successfully."),
    ]
    public bool Version { get; set; }
}

internal class ExistsValidatorAttribute : ValidatorAttribute<string>
{
    public override string ErrorMessage { get; }

    public override bool IsValid(string? arg) => File.Exists(arg);
}

internal static class Program
{
    public static void Main(string[] args)
    {
        ArgParser<Args> argParser = ArgParserFactory.FromType<Args>();
        Args arguments = argParser.Parse(args);
    }
}
