using ArgParser.Internal.Metadata;

namespace ArgParser.Internal.Arguments;

internal record ArgOccurence(string Name, PropertyMetadata Property);

internal class CoupledArgs
{
    internal required List<(ArgOccurence, string?)> Couples { get; init; }
    internal required List<ArgOccurence> Flags { get; init; }
    internal required List<string> Rest { get; init; }

    private static bool TryGetLongOptionValue(
        string option,
        ProcessedClassMetadata metadata,
        out (ArgOccurence?, string?) occurence
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

        occurence = (new ArgOccurence(optionName, property), strValue);
        return true;
    }

    internal static CoupledArgs FromArgs(string[] args, ProcessedClassMetadata metadata)
    {
        List<(ArgOccurence, string?)> couples = new();
        List<ArgOccurence> flags = new();
        List<string> rest = new();

        Queue<string> queuedArgs = new(args);

        while (queuedArgs.Count > 0)
        {
            string arg = queuedArgs.Dequeue();

            if (metadata.NamesToFlag.TryGetValue(arg, out PropertyMetadata? flag))
            {
                flags.Add(new ArgOccurence(arg, flag));
                continue;
            }

            if (metadata.ShortNamesToOption.TryGetValue(arg, out PropertyMetadata? option))
            {
                string? val = queuedArgs.Count > 0 ? null : queuedArgs.Dequeue();
                couples.Add((new ArgOccurence(arg, option), val));
                continue;
            }

            if (TryGetLongOptionValue(arg, metadata, out (ArgOccurence?, string?) occurence))
            {
                couples.Add(occurence!);
                continue;
            }

            rest.Add(arg);
            if (arg == "--")
                break;
        }

        return new CoupledArgs
        {
            Couples = couples,
            Flags = flags,
            Rest = rest.Concat(queuedArgs).ToList(),
        };
    }
}
