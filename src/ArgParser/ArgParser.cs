using ArgParser.Attributes;
using ArgParser.Exceptions;
using ArgParser.Internal;
using ArgParser.Internal.Arguments;
using ArgParser.Internal.Metadata;

namespace ArgParser;

/// <summary>
/// Base class your declared arguments class should inherit from.
/// </summary>
public abstract class BaseArgs
{
    /// <summary>
    /// The default implementation of the help flag.
    /// </summary>
    [ShortNames('h'), LongNames("help"), TerminatingFlag<HelpCalledException>]
    public virtual bool HelpCalled { get; set; }

    /// <summary>
    /// Where default program arguments will be stored.
    /// </summary>
    public abstract string[] PlainArguments { get; set; }
}

/// <summary>
/// Implements methods for creating <see cref="ArgParser{TArgs}"/>.
/// </summary>
public static class ArgParserFactory
{
    /// <summary>
    /// Creates a new <see cref="ArgParser{TArgs}"/>.
    /// </summary>
    /// <typeparam name="TArgs">Type of the created ArgParser</typeparam>
    /// <exception cref="ParserConfigurationException">Describes errors encountered during the validation of TArgs</exception>
    public static ArgParser<TArgs> FromType<TArgs>()
        where TArgs : BaseArgs, new()
    {
        Type ArgType = typeof(TArgs);

        ArgsClassMetadata classMetadata = ArgsClassMetadata.FromType(ArgType);
        MetadataValidator.Validate(classMetadata);

        ProcessedClassMetadata processed = ProcessedClassMetadata.FromMetadata(classMetadata);
        return new ArgParser<TArgs>(processed);
    }
}

/// <summary>
/// Implements parsing of standard program command line arguments into the specified type
/// </summary>
/// <typeparam name="TArgs">Declared argument type</typeparam>
public sealed class ArgParser<TArgs>
    where TArgs : BaseArgs, new()
{
    private readonly ProcessedClassMetadata _metadata;

    internal ArgParser(ProcessedClassMetadata metadata)
    {
        _metadata = metadata;
    }

    private static void CheckTerminatingFlags(List<ArgOccurence> flags)
    {
        for (int i = flags.Count - 1; i >= 0; i--)
        {
            if (flags[i].Property.Behavior.TerminatingFlag is ITerminatingFlag t)
                t.ThrowException();
        }
    }

    private static void CheckUnknownArguments(List<string> beforeDelimiter)
    {
        foreach (string plainArg in beforeDelimiter)
        {
            if (plainArg.StartsWith('-'))
                throw new UnknownOptionException($"Unknown option '{plainArg}'");
        }
    }

    private static void CheckMissingOptionValues(List<(ArgOccurence, string?)> coupled)
    {
        foreach ((ArgOccurence occurence, string? value) in coupled)
        {
            if (value is null)
                throw new MissingOptionValueException(
                    $"Missing option value for '{occurence.Name}'"
                );
        }
    }

    private static void CheckDuplicateOccurences(List<(ArgOccurence, string?)> couples, List<ArgOccurence> flags)
    {
        Dictionary<PropertyMetadata, string> occurences = new();

        foreach (ArgOccurence flag in flags)
        {
            if (occurences.TryGetValue(flag.Property, out string? firstOccurence))
                throw new DuplicateOccurrenceException(
                    $"Flag '{flag.Name}' was specified before as '{firstOccurence}'."
                );
            occurences[flag.Property] = flag.Name;
        }

        foreach ((ArgOccurence occurence, _) in couples)
        {
            if (occurences.TryGetValue(occurence.Property, out string? firstOccurence))
                throw new DuplicateOccurrenceException(
                    $"Option '{occurence.Name}' was specified before as '{firstOccurence}'."
                );
            occurences[occurence.Property] = occurence.Name;
        }
    }

    /// <summary>
    /// Tries parsing the command line arguments according to the structure of
    /// TArgs and attributes defined in it.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <exception cref="CommandLineParsingException">Argument parsing has failed</exception>
    /// <exception cref="HelpCalledException">Program was called with the 'help' terminating flag</exception>
    public TArgs Parse(string[] args)
    {
        CoupledArgs coupled = CoupledArgs.FromArgs(args, _metadata);

        CheckTerminatingFlags(coupled.Flags);
        CheckUnknownArguments(coupled.PlainBeforeDelimiter);
        CheckMissingOptionValues(coupled.Couples);
        CheckDuplicateOccurences(coupled.Couples, coupled.Flags);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a help message based on the information provided in attributes:
    /// <see cref="HelpAttribute"/>,
    /// <br/>
    /// <see cref="MetaVarNameAttribute"/>, and
    /// <br/>
    /// <see cref="ExampleUsageAttribute"/>.
    /// </summary>
    public string GenerateHelpMessage()
    {
        return "";
    }
}
