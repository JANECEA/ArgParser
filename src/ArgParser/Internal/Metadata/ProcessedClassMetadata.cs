namespace ArgParser.Internal.Metadata;

internal class ProcessedClassMetadata
{
    internal required Dictionary<string, ITerminatingFlag> TerminatingFlags { get; init; }
    internal required Dictionary<string, PropertyMetadata> LongNamesToProperty { get; init; }
    internal required Dictionary<string, PropertyMetadata> ShortNamesToProperty { get; init; }

    internal ArgsClassMetadata ClassMetadata { get; }

    private ProcessedClassMetadata(ArgsClassMetadata classMetadata) =>
        ClassMetadata = classMetadata;

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

    private static Dictionary<string, PropertyMetadata> GetLongNamesToProperty(
        ArgsClassMetadata metadata
    )
    {
        Dictionary<string, PropertyMetadata> longNamesToProperty = new();
        foreach (PropertyMetadata p in metadata.Properties)
        {
            foreach (string longName in p.Behavior.LongNames)
                longNamesToProperty.Add($"--{longName}", p);
        }
        return longNamesToProperty;
    }

    private static Dictionary<string, PropertyMetadata> GetShortNamesToProperty(
        ArgsClassMetadata metadata
    )
    {
        Dictionary<string, PropertyMetadata> shortNamesToProperty = new();
        foreach (PropertyMetadata p in metadata.Properties)
        {
            foreach (char shortName in p.Behavior.ShortNames)
                shortNamesToProperty.Add($"-{shortName}", p);
        }
        return shortNamesToProperty;
    }

    internal static ProcessedClassMetadata FromMetadata(ArgsClassMetadata metadata) =>
        new(metadata)
        {
            TerminatingFlags = GetTerminatingFlags(metadata),
            LongNamesToProperty = GetLongNamesToProperty(metadata),
            ShortNamesToProperty = GetShortNamesToProperty(metadata),
        };
}
