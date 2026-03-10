using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal HelpCalledException(string? message)
        : base(message) { }
}
