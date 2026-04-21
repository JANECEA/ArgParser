using ArgParser.Internal.Metadata;

namespace ArgParser.Internal.Arguments;

internal record ArgOccurrence(string Name, PropertyMetadata Property);

internal class CoupledArgs
{
    internal required List<(ArgOccurrence, string?)> Couples { get; init; }
    internal required List<ArgOccurrence> Flags { get; init; }
    internal required List<string> PlainBeforeDelimiter { get; init; }
    internal required List<string> PlainAfterDelimiter { get; init; }

    private static bool TryGetLongOptionValue(
        string option,
        ProcessedClassMetadata metadata,
        out (ArgOccurrence?, string?) occurence
    )
    {
        occurence = (null, null);
        string optionName = option;

        int index = option.IndexOf('=');
        if (index != -1)
            optionName = option[..index];

        if (!metadata.LongNamesToOption.TryGetValue(optionName, out PropertyMetadata? property))
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
        List<string> rest = new();

        Queue<string> queuedArgs = new(args);

        while (queuedArgs.Count > 0)
        {
            string arg = queuedArgs.Dequeue();

            if (metadata.NamesToFlag.TryGetValue(arg, out PropertyMetadata? flag))
            {
                flags.Add(new ArgOccurrence(arg, flag));
                continue;
            }

            if (metadata.ShortNamesToOption.TryGetValue(arg, out PropertyMetadata? option))
            {
                string? val = queuedArgs.Count > 0 ? null : queuedArgs.Dequeue();
                couples.Add((new ArgOccurrence(arg, option), val));
                continue;
            }

            if (TryGetLongOptionValue(arg, metadata, out (ArgOccurrence?, string?) occurence))
            {
                couples.Add(occurence!);
                continue;
            }

            if (arg == "--")
                break;
            rest.Add(arg);
        }

        return new CoupledArgs
        {
            Couples = couples,
            Flags = flags,
            PlainBeforeDelimiter = rest,
            PlainAfterDelimiter = queuedArgs.ToList(),
        };
    }
}
