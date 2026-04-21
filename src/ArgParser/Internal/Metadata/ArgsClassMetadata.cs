using System.Reflection;
using ArgParser.Attributes;

namespace ArgParser.Internal.Metadata;

internal class ArgsClassMetadata
{
    internal required Type ClassType { get; init; }
    internal required List<PropertyMetadata> Properties { get; init; }
    internal required string ExampleUsage { get; init; }
    internal required List<IClassValidator> Validators { get; init; }

    private static List<IClassValidator> GetValidators(Type classType) =>
        classType.GetCustomAttributes(false).OfType<IClassValidator>().ToList();

    internal static ArgsClassMetadata FromType(Type type)
    {
        PropertyInfo[] properties = type.GetProperties();

        List<PropertyMetadata> metadata = properties
            .Select(PropertyMetadata.FromPropertyInfo)
            .Where(m => m.HasLongOrShortNames())
            .ToList();

        return new ArgsClassMetadata
        {
            ClassType = type,
            ExampleUsage =
                type.GetCustomAttribute<ExampleUsageAttribute>(false)?.Usage ?? string.Empty,
            Validators = GetValidators(type),
            Properties = metadata,
        };
    }
}
