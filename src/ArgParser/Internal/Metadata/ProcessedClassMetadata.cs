using System.Diagnostics;

namespace ArgParser.Internal.Metadata;

internal class ProcessedClassMetadata
{
    internal required IReadOnlyList<PropertyMetadata> AllFlags { get; init; }
    internal required IReadOnlyDictionary<string, PropertyMetadata> NamesToFlag { get; init; }
    internal required IReadOnlyList<PropertyMetadata> AllOptions { get; init; }
    internal required IReadOnlyDictionary<string, PropertyMetadata> NamesToOption { get; init; }
    internal required IReadOnlyList<PropertyMetadata> AllArguments { get; init; }
    internal required IReadOnlyList<IClassValidator> ClassValidators { get; init; }

    private static Dictionary<string, PropertyMetadata> GetNamesToMetadata(
        ArgsClassMetadata metadata,
        Predicate<PropertyMetadata> predicate
    )
    {
        Dictionary<string, PropertyMetadata> namesToMetadata = new();

        foreach (PropertyMetadata p in metadata.Options.Where(p => predicate(p)))
        {
            foreach (char shortName in p.Behavior.ShortNames)
                namesToMetadata.Add(CliStandards.GetShortName(shortName), p);

            foreach (string longName in p.Behavior.LongNames)
                namesToMetadata.Add(CliStandards.GetLongName(longName), p);
        }

        return namesToMetadata;
    }

    private static List<PropertyMetadata> OrderArguments(ArgsClassMetadata metadata)
    {
        List<PropertyMetadata> orderedArgs = new();
        Dictionary<string, PropertyMetadata> args = metadata.Arguments.ToDictionary(
            a => a.Info.Name,
            a => a
        );

        foreach (string propertyName in metadata.PositionalArgs)
            if (args.TryGetValue(propertyName, out PropertyMetadata? meta))
                orderedArgs.Add(meta);
            else
                throw new UnreachableException(
                    $"Presence of property {propertyName} should have been validated."
                );

        return orderedArgs;
    }

    internal static ProcessedClassMetadata FromMetadata(ArgsClassMetadata metadata) =>
        new()
        {
            AllFlags = metadata.Options.Where(m => m.IsFlag()).ToList(),
            NamesToFlag = GetNamesToMetadata(metadata, p => p.IsFlag()),
            AllOptions = metadata.Options.Where(m => !m.IsFlag()).ToList(),
            NamesToOption = GetNamesToMetadata(metadata, p => !p.IsFlag()),
            AllArguments = OrderArguments(metadata),
            ClassValidators = metadata.Validators,
        };
}
