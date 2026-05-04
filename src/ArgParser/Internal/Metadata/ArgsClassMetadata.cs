using System.Reflection;
using ArgParser.Attributes;

namespace ArgParser.Internal.Metadata;

internal class ArgsClassMetadata
{
    internal required Type ClassType { get; init; }
    internal required IReadOnlyList<PropertyMetadata> Options { get; init; }
    internal required IReadOnlyList<PropertyMetadata> Arguments { get; init; }
    internal required string ExampleUsage { get; init; }
    internal required IReadOnlyList<IClassValidator> Validators { get; init; }
    internal required IReadOnlyList<string> PositionalArgs { get; init; }

    private static List<IClassValidator> GetValidators(Type classType) =>
        classType.GetCustomAttributes(false).OfType<IClassValidator>().ToList();

    private static (List<PropertyMetadata>, List<PropertyMetadata>) FilterProperties(
        PropertyMetadata[] properties,
        List<string> positionalArgs
    )
    {
        List<PropertyMetadata> options = new();
        List<PropertyMetadata> arguments = new();
        foreach (PropertyMetadata property in properties)
        {
            if (property.HasLongOrShortNames())
                options.Add(property);
            if (positionalArgs.Contains(property.Info.Name))
                arguments.Add(property);
        }

        return (options, arguments);
    }

    internal static ArgsClassMetadata FromType(Type type)
    {
        PropertyMetadata[] properties = type.GetProperties()
            .Select(PropertyMetadata.FromPropertyInfo)
            .ToArray();

        List<string> positionalArgs =
            type.GetCustomAttribute<PositionalArgsAttribute>(false)?.PropertyNames ?? [];
        var (options, arguments) = FilterProperties(properties, positionalArgs);

        return new ArgsClassMetadata
        {
            ClassType = type,
            ExampleUsage =
                type.GetCustomAttribute<ExampleUsageAttribute>(false)?.Usage ?? string.Empty,
            Validators = GetValidators(type),
            Options = options,
            Arguments = arguments,
            PositionalArgs = positionalArgs,
        };
    }
}
