using ArgParser.Exceptions;
using ArgParser.Internal.Metadata;

namespace ArgParser.Internal.CommandLine;

internal static class CommandLineValidator
{
    internal static void CheckUnknownArguments(IReadOnlyList<string> beforeDelimiter)
    {
        foreach (string plainArg in beforeDelimiter)
        {
            if (plainArg.StartsWith('-'))
                throw new UnknownOptionException($"Unknown option '{plainArg}'");
        }
    }

    internal static void CheckMissingOptionValues(IReadOnlyList<(ArgOccurrence, string?)> coupled)
    {
        foreach ((ArgOccurrence occurence, string? value) in coupled)
        {
            if (value is null)
                throw new MissingOptionValueException(
                    $"Missing option value for '{occurence.Name}'"
                );
        }
    }

    internal static void CheckDuplicateOccurrences(
        IReadOnlyList<(ArgOccurrence, string?)> couples,
        IReadOnlyList<ArgOccurrence> flags
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

    internal static void CheckRequired(
        Dictionary<PropertyMetadata, object> foundValues,
        ProcessedClassMetadata metadata
    )
    {
        foreach (PropertyMetadata property in metadata.AllOptions.Concat(metadata.AllArguments))
        {
            if (!property.Behavior.IsRequired)
                continue;

            if (!foundValues.ContainsKey(property))
                throw new MissingRequiredOptionException(
                    $"Option '{property.Info.Name}' is marked as required, but was not given"
                );
        }
    }

    internal static void CheckRequires(Dictionary<PropertyMetadata, object> foundValues)
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

    internal static void ApplyOptionValidators(Dictionary<PropertyMetadata, object> foundValues)
    {
        foreach ((PropertyMetadata property, object value) in foundValues)
        foreach (IOptionValidator v in property.Validators)
            if (!v.ValidateInternal(value, out string? errorMessage))
                throw new ValidatorFailedException(errorMessage);
    }

    internal static void ApplyClassValidators<TArgs>(
        TArgs argObject,
        ProcessedClassMetadata metadata
    )
        where TArgs : BaseArgs
    {
        foreach (IClassValidator v in metadata.ClassValidators)
            if (!v.ValidateInternal(argObject, out string? errorMessage))
                throw new ValidatorFailedException(errorMessage);
    }
}
