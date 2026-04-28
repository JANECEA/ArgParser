using ArgParser.Internal.Metadata;

namespace ArgParser.Internal.CommandLine;

internal record ArgOccurrence(string Name, PropertyMetadata Property);

internal class CoupledArgs
{
    internal required IReadOnlyList<(ArgOccurrence, string?)> Couples { get; init; }
    internal required IReadOnlyList<ArgOccurrence> Flags { get; init; }
    internal required IReadOnlyList<string> PlainBeforeDelimiter { get; init; }
    internal required IReadOnlyList<string> PlainAfterDelimiter { get; init; }

    private static bool TryGetOptionValue(
        string option,
        ProcessedClassMetadata metadata,
        out (ArgOccurrence, string?)? occurence
    )
    {
        occurence = null;

        string optionName = option;
        int index = option.IndexOf('=');
        if (index != -1)
            optionName = option[..index];

        if (!metadata.NamesToOption.TryGetValue(optionName, out PropertyMetadata? property))
            return false;

        string? strValue;
        if (index == -1)
            strValue = null;
        else
            strValue = index == option.Length - 1 ? string.Empty : option[(index + 1)..];

        occurence = (new ArgOccurrence(optionName, property), strValue);
        return true;
    }

    internal static CoupledArgs FromArgs(string[] args, ProcessedClassMetadata metadata)
    {
        List<(ArgOccurrence, string?)> couples = new();
        List<ArgOccurrence> flags = new();
        List<string> beforeDelimiter = new();

        Queue<string> queuedArgs = new(args);

        while (queuedArgs.Count > 0)
        {
            string arg = queuedArgs.Dequeue();

            if (metadata.NamesToFlag.TryGetValue(arg, out PropertyMetadata? flag))
            {
                flags.Add(new ArgOccurrence(arg, flag));
                continue;
            }

            if (TryGetOptionValue(arg, metadata, out (ArgOccurrence, string?)? occurence))
            {
                (ArgOccurrence name, string? value) couple = occurence!.Value;
                if (couple.value is null && queuedArgs.Count > 0)
                    couple.value = queuedArgs.Dequeue();

                couples.Add(couple);
                continue;
            }

            if (arg == "--")
                break;
            beforeDelimiter.Add(arg);
        }

        return new CoupledArgs
        {
            Couples = couples,
            Flags = flags,
            PlainBeforeDelimiter = beforeDelimiter,
            PlainAfterDelimiter = queuedArgs.ToList(),
        };
    }
}
