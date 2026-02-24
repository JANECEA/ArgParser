using ArgParser;

namespace ExampleUsage;

class Args : BaseArgs
{
    [ShortOptions("-f")]
    [LongOptions("--format")]
    [Help(
        """
            Specify output format, possibly overriding the format specified
            in the environment variable TIME
            """
    )]
    public string? Format { get; set; }

    [ShortOptions("-p")]
    [LongOptions("--portability")]
    [Help("Use the protable output format")]
    public bool Portability { get; set; }

    [ShortOptions("-o")]
    [LongOptions("--output")]
    [Help("Do not send the results to stderr, but overwrite the specified file.")]
    public string? Output { get; set; }

    [ShortOptions("-a")]
    [LongOptions("--append")]
    [Require(nameof(Output))]
    [Help("(Used together with -o.) Do not overwrite but append.")]
    public bool Append { get; set; }

    [ShortOptions("-v")]
    [LongOptions("--verbose")]
    [Help("Give very verbose output about all the program knows about.")]
    public bool Verbose { get; set; }

    [ShortOptions("-V")]
    [LongOptions("--version")]
    [Help("Print version information on standard output, then exit successfully.")]
    public bool Version { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        Parser<Args> parser = new();
        Args arg = parser.Parse(args);
    }
}
