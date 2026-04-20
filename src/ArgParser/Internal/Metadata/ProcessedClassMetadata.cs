namespace ArgParser.Internal.Metadata;

internal class ProcessedClassMetadata
{
    internal required Dictionary<string, PropertyMetadata> NamesToFlag { get; init; }
    internal required Dictionary<string, PropertyMetadata> LongNamesToOption { get; init; }
    internal required Dictionary<string, PropertyMetadata> ShortNamesToOption { get; init; }

    internal ArgsClassMetadata ClassMetadata { get; }

    private ProcessedClassMetadata(ArgsClassMetadata classMetadata) =>
        ClassMetadata = classMetadata;

    private static Dictionary<string, PropertyMetadata> GetNamesToFlag(
        ArgsClassMetadata metadata
    )
    {
        Dictionary<string, PropertyMetadata> namesToFlag = new();
        foreach (PropertyMetadata p in metadata.Properties)
        {
            if (!p.IsFlag())
                continue;
            BehaviorMetadata behavior = p.Behavior;

            foreach (string longName in behavior.LongNames)
                namesToFlag.Add($"--{longName}", p);

            foreach (char shortName in behavior.ShortNames)
                namesToFlag.Add($"-{shortName}", p);
        }
        return namesToFlag;
    }

    private static Dictionary<string, PropertyMetadata> GetLongNamesToOption(
        ArgsClassMetadata metadata
    )
    {
        Dictionary<string, PropertyMetadata> longNamesToProperty = new();
        foreach (PropertyMetadata p in metadata.Properties)
        {
            if(p.IsFlag()) 
                continue;

            foreach (string longName in p.Behavior.LongNames)
                longNamesToProperty.Add($"--{longName}", p);
        }
        return longNamesToProperty;
    }

    private static Dictionary<string, PropertyMetadata> GetShortNamesToOption(
        ArgsClassMetadata metadata
    )
    {
        Dictionary<string, PropertyMetadata> shortNamesToProperty = new();
        foreach (PropertyMetadata p in metadata.Properties)
        {
            if (p.IsFlag())
                continue;

            foreach (char shortName in p.Behavior.ShortNames)
                shortNamesToProperty.Add($"-{shortName}", p);
        }
        return shortNamesToProperty;
    }

    internal static ProcessedClassMetadata FromMetadata(ArgsClassMetadata metadata) =>
        new(metadata)
        {
            NamesToFlag = GetNamesToFlag(metadata),
            LongNamesToOption = GetLongNamesToOption(metadata),
            ShortNamesToOption = GetShortNamesToOption(metadata),
        };
}
