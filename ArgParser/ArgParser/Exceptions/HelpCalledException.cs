namespace ArgParser.Exceptions;

/// <summary>
/// The exception that is thrown while parsing command line arguments if help option was given ("--help", "-h").
/// </summary>
public sealed class HelpCalledException : Exception
{
    /// <summary>
    /// Property that contains the generated help message.
    /// </summary>
    public string HelpMessage { get; internal set; } = string.Empty;

    public HelpCalledException()
        : base("Program has been called with the 'help' flag.") { }
}
