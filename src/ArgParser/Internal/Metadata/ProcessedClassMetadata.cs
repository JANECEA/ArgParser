namespace ArgParser.Internal.Metadata;

internal class ProcessedClassMetadata
{
    internal required IReadOnlyList<PropertyMetadata> AllFlags { get; init; }
    internal required IReadOnlyDictionary<string, PropertyMetadata> NamesToFlag { get; init; }
    internal required IReadOnlyList<PropertyMetadata> AllOptions { get; init; }
    internal required IReadOnlyDictionary<string, PropertyMetadata> NamesToOption { get; init; }
    internal required IReadOnlyList<IClassValidator> ClassValidators { get; init; }

    private static Dictionary<string, PropertyMetadata> GetNamesToMetadata(
        ArgsClassMetadata metadata,
        Predicate<PropertyMetadata> predicate
    )
    {
        Dictionary<string, PropertyMetadata> namesToMetadata = new();

        foreach (PropertyMetadata p in metadata.Options.Where(p => predicate(p)))
        {
            foreach (string longName in p.Behavior.LongNames)
                namesToMetadata.Add($"--{longName}", p);

            foreach (char shortName in p.Behavior.ShortNames)
                namesToMetadata.Add($"-{shortName}", p);
        }

        return namesToMetadata;
    }

    internal static ProcessedClassMetadata FromMetadata(ArgsClassMetadata metadata) =>
        new()
        {
            AllFlags = metadata.Options.Where(m => m.IsFlag()).ToList(),
            NamesToFlag = GetNamesToMetadata(metadata, p => p.IsFlag()),
            AllOptions = metadata.Options.Where(m => !m.IsFlag()).ToList(),
            NamesToOption = GetNamesToMetadata(metadata, p => !p.IsFlag()),
            ClassValidators = metadata.Validators,
        };
}
