namespace ArgParser.Exceptions;

/// <summary>
/// The exception that is thrown while parsing command line arguments if help option was given ("--help", "-h").
/// </summary>
public sealed class HelpCalledException : Exception
{
    /// <summary>
    /// Creates a new instance of <see cref="HelpCalledException"/>
    /// </summary>
    public HelpCalledException()
        : base("Program has been called with the 'help' flag.") { }
}
