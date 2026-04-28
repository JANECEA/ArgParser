using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace ArgParser;

/// <summary>
/// Base class your declared arguments class should inherit from.
/// </summary>
public abstract class BaseArgs
{
    /// <summary>
    /// The default implementation of the help flag.
    /// </summary>
    [
        ShortNames('h'),
        LongNames("help"),
        Help("Prints help message and exits."),
        TerminatingFlag<HelpCalledException>,
    ]
    public virtual bool HelpCalled { get; set; }

    /// <summary>
    /// Where default program arguments will be stored.
    /// </summary>
    public abstract string[] PlainArguments { get; set; }
}
