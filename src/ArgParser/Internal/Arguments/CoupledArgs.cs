using ArgParser.Internal.Metadata;

namespace ArgParser.Internal.Arguments;

internal class CoupledArgs
{
    internal required List<(string, string?)> Couples { get; init; } = new();
    internal required List<string> Flags { get; init; } = new();
    internal required List<string> Rest { get; init; } = new();

    private static bool TryGetLongOptionValue(
        string option,
        ProcessedClassMetadata metadata,
        out string? value
    )
    {
        value = null;
        if (metadata.LongNamesToOption.ContainsKey(option))
            return true;

        int index = option.IndexOf('=');
        if (index == -1)
            return false;

        string optionName = option[..index];
        value = index == option.Length - 1 ? string.Empty : option[(index + 1)..];
        return metadata.LongNamesToOption.ContainsKey(optionName);
    }

    internal static CoupledArgs FromArgs(string[] args, ProcessedClassMetadata metadata)
    {
        List<(string, string?)> couples = new();
        List<string> flags = new();
        List<string> rest = new();

        Queue<string> queuedArgs = new(args);

        while (queuedArgs.Count > 0)
        {
            string arg = queuedArgs.Dequeue();

            if (metadata.NamesToFlag.ContainsKey(arg))
            {
                flags.Add(arg);
                continue;
            }

            if (metadata.ShortNamesToOption.ContainsKey(arg))
            {
                string? val = queuedArgs.Count > 0 ? null : queuedArgs.Dequeue();
                couples.Add((arg, val));
                continue;
            }

            if (TryGetLongOptionValue(arg, metadata, out string? value))
            {
                couples.Add((arg, value));
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
