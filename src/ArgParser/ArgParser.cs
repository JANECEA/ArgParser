using System.Diagnostics;
using System.Reflection;
using ArgParser.Attributes;
using ArgParser.Exceptions;
using ArgParser.Internal;
using ArgParser.Internal.CommandLine;
using ArgParser.Internal.Metadata;
using ArgParser.Internal.Parsing;

namespace ArgParser;

/// <summary>
/// Implements parsing of standard program command line arguments into the specified type
/// </summary>
/// <typeparam name="TArgs">Declared argument type</typeparam>
public sealed class ArgParser<TArgs>
    where TArgs : BaseArgs, new()
{
    private readonly ProcessedClassMetadata _metadata;
    private readonly Lazy<string> _helpMessage;

    internal ArgParser(ProcessedClassMetadata metadata, Lazy<string> helpMessage)
    {
        _metadata = metadata;
        _helpMessage = helpMessage;
    }

    private static void CheckTerminatingFlags(IReadOnlyList<ArgOccurrence> flags)
    {
        for (int i = flags.Count - 1; i >= 0; i--)
        {
            if (flags[i].Property.Behavior.TerminatingFlag is ITerminatingFlag t)
                t.ThrowException();
        }
    }

    private void ResetFlagValues(TArgs argsObject)
    {
        foreach (PropertyMetadata flag in _metadata.AllFlags)
            flag.Info.SetValue(argsObject, false);
    }

    private static void SetAllValues(
        TArgs argsObject,
        Dictionary<PropertyMetadata, object> foundValues
    )
    {
        foreach ((PropertyMetadata property, object value) in foundValues)
            property.Info.SetValue(argsObject, value);
    }

    private TArgs GetArgObject(
        CoupledArgs coupled,
        Dictionary<PropertyMetadata, object> foundValues
    )
    {
        TArgs argObject = new()
        {
            HelpCalled = false,
            PlainArguments = coupled
                .PlainBeforeDelimiter.Concat(coupled.PlainAfterDelimiter)
                .ToArray(),
        };
        ResetFlagValues(argObject);
        SetAllValues(argObject, foundValues);
        return argObject;
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

        CommandLineValidator.CheckUnknownArguments(coupled.PlainBeforeDelimiter);
        CommandLineValidator.CheckMissingOptionValues(coupled.Couples);
        CommandLineValidator.CheckDuplicateOccurrences(coupled.Couples, coupled.Flags);

        Dictionary<PropertyMetadata, object> foundValues = ValueParser.GetFoundValues(coupled);

        CommandLineValidator.CheckRequired(foundValues, _metadata);
        CommandLineValidator.CheckRequires(foundValues);
        CommandLineValidator.ApplyOptionValidators(foundValues);

        TArgs argObject = GetArgObject(coupled, foundValues);

        CommandLineValidator.ApplyClassValidators(argObject, _metadata);
        return argObject;
    }

    /// <summary>
    /// Generates a help message based on the information provided in attributes:
    /// <see cref="HelpAttribute"/>,
    /// <br/>
    /// <see cref="MetaVarNameAttribute"/>, and
    /// <br/>
    /// <see cref="ExampleUsageAttribute"/>.
    /// </summary>
    public string GenerateHelpMessage() => _helpMessage.Value;
}
