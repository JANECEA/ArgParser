using System.Diagnostics;
using System.Reflection;
using ArgParser.Attributes;
using ArgParser.Exceptions;
using ArgParser.Internal;
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

    private static void CheckTerminatingFlags(List<ArgOccurrence> flags)
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

    private static void CheckMissingOptionValues(List<(ArgOccurrence, string?)> coupled)
    {
        foreach ((ArgOccurrence occurence, string? value) in coupled)
        {
            if (value is null)
                throw new MissingOptionValueException(
                    $"Missing option value for '{occurence.Name}'"
                );
        }
    }

    private static void CheckDuplicateOccurrences(
        List<(ArgOccurrence, string?)> couples,
        List<ArgOccurrence> flags
    )
    {
        Dictionary<PropertyMetadata, string> occurrences = new();

        foreach (ArgOccurrence flag in flags)
        {
            if (occurrences.TryGetValue(flag.Property, out string? firstOccurence))
                throw new DuplicateOccurrenceException(
                    $"Flag '{flag.Name}' was specified before as '{firstOccurence}'."
                );
            occurrences[flag.Property] = flag.Name;
        }

        foreach ((ArgOccurrence occurence, _) in couples)
        {
            if (occurrences.TryGetValue(occurence.Property, out string? firstOccurence))
                throw new DuplicateOccurrenceException(
                    $"Option '{occurence.Name}' was specified before as '{firstOccurence}'."
                );
            occurrences[occurence.Property] = occurence.Name;
        }
    }

    private void ResetFlagValues(TArgs argsObject)
    {
        foreach (PropertyMetadata flag in _metadata.AllFlags)
            flag.Info.SetValue(argsObject, false);
    }

    private static Dictionary<PropertyMetadata, object> ParseOptionValues(
        List<(ArgOccurrence, string?)> foundOptions
    )
    {
        Dictionary<PropertyMetadata, object> foundValues = new();

        foreach ((ArgOccurrence occurence, string? strValue) in foundOptions)
        {
            if (strValue is null)
                throw new UnreachableException(
                    "Missing option values should have been caught by CheckMissingOptionValues."
                );

            Type targetType = occurence.Property.Info.PropertyType;
            Type parseType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            MethodInfo? parseMethod = parseType.GetMethod(
                "Parse",
                BindingFlags.Public | BindingFlags.Static,
                [typeof(string), typeof(IFormatProvider)]
            );
            if (parseMethod is null)
                throw new UnreachableException(
                    "Internal error: Type does not implement IParsable<T>."
                );

            object parsedValue;
            try
            {
                parsedValue = parseMethod.Invoke(null, [strValue, null])!;
            }
            catch
            {
                throw new ValueParsingException(
                    $"Could not parse value '{strValue}' into type {parseType.Name}."
                );
            }

            foundValues[occurence.Property] = parsedValue;
        }
        return foundValues;
    }

    private void CheckRequired(Dictionary<PropertyMetadata, object> foundValues)
    {
        foreach (PropertyMetadata property in _metadata.AllOptions)
        {
            if (!property.Behavior.IsRequired)
                continue;

            if (!foundValues.ContainsKey(property))
                throw new MissingRequiredOptionException(
                    $"Option '{property.Info.Name}' is marked as required, but was not given"
                );
        }
    }

    private static void SetAllValues(
        TArgs argsObject,
        Dictionary<PropertyMetadata, object> foundValues
    )
    {
        foreach ((PropertyMetadata property, object value) in foundValues)
            property.Info.SetValue(argsObject, value);
    }

    private static void CheckRequires(Dictionary<PropertyMetadata, object> foundValues)
    {
        HashSet<string> foundNames = foundValues.Keys.Select(p => p.Info.Name).ToHashSet();

        foreach (PropertyMetadata property in foundValues.Keys)
        {
            if (property.Behavior.Requires.Count == 0)
                continue;

            foreach (string requiredName in property.Behavior.Requires)
            {
                if (!foundNames.Contains(requiredName))
                    throw new MissingRequiredOptionException(
                        $"Option '{property.Info.Name}' requires '{requiredName}' to be specified, but it was not."
                    );
            }
        }
    }

    private static void AddFoundFlags(
        Dictionary<PropertyMetadata, object> foundValues,
        List<ArgOccurrence> foundFlags
    )
    {
        foreach (ArgOccurrence flag in foundFlags)
            foundValues[flag.Property] = true;
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
        CheckDuplicateOccurrences(coupled.Couples, coupled.Flags);

        Dictionary<PropertyMetadata, object> foundValues = ParseOptionValues(coupled.Couples);
        AddFoundFlags(foundValues, coupled.Flags);

        CheckRequired(foundValues);
        CheckRequires(foundValues);

        foreach ((PropertyMetadata property, object value) in foundValues)
        foreach (IOptionValidator v in property.Validators)
            if (!v.ValidateInternal(value, out string? errorMessage))
                throw new ValidatorFailedException(errorMessage);

        TArgs argObject = new()
        {
            HelpCalled = false,
            PlainArguments = coupled
                .PlainBeforeDelimiter.Concat(coupled.PlainAfterDelimiter)
                .ToArray(),
        };
        ResetFlagValues(argObject);
        SetAllValues(argObject, foundValues);

        foreach (IClassValidator v in _metadata.ClassValidators)
            if (!v.ValidateInternal(argObject, out string? errorMessage))
                throw new ValidatorFailedException(errorMessage);

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
