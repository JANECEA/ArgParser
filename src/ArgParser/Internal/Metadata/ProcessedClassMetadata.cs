namespace ArgParser.Internal.Metadata;

internal class ProcessedClassMetadata
{
    internal required Dictionary<string, ITerminatingFlag> TerminatingFlags { get; init; }
    internal ArgsClassMetadata ClassMetadata { get; }

    private ProcessedClassMetadata(ArgsClassMetadata classMetadata)
    {
        ClassMetadata = classMetadata;
    }

    private static Dictionary<string, ITerminatingFlag> GetTerminatingFlags(
        ArgsClassMetadata metadata
    )
    {
        Dictionary<string, ITerminatingFlag> terminatingFlags = new();
        foreach (PropertyMetadata p in metadata.Properties)
        {
            BehaviorMetadata behavior = p.Behavior;
            if (behavior.TerminatingFlag is null)
                continue;

            foreach (char shortName in behavior.ShortNames)
                terminatingFlags.Add($"-{shortName}", behavior.TerminatingFlag);

            foreach (string longName in behavior.LongNames)
                terminatingFlags.Add($"--{longName}", behavior.TerminatingFlag);
        }

        return terminatingFlags;
    }

    internal static ProcessedClassMetadata FromMetadata(ArgsClassMetadata metadata)
    {
        return new ProcessedClassMetadata(metadata)
        {
            TerminatingFlags = GetTerminatingFlags(metadata),
        };
    }
}
