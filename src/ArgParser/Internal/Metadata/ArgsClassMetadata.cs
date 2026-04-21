using System.Reflection;
using ArgParser.Attributes;

namespace ArgParser.Internal.Metadata;

internal class BehaviorMetadata
{
    internal required List<char> ShortNames { get; init; }
    internal required List<string> LongNames { get; init; }
    internal required bool IsRequired { get; init; }
    internal required List<string> Requires { get; init; }
    internal required ITerminatingFlag? TerminatingFlag { get; init; }

    private static ITerminatingFlag? GetTerminatingFlag(PropertyInfo propertyInfo) =>
        propertyInfo.GetCustomAttributes(false).OfType<ITerminatingFlag>().FirstOrDefault();

    internal static BehaviorMetadata FromPropertyInfo(PropertyInfo propertyInfo) =>
        new()
        {
            ShortNames = propertyInfo.GetCustomAttribute<ShortNamesAttribute>(false)?.Names ?? [],
            LongNames = propertyInfo.GetCustomAttribute<LongNamesAttribute>(false)?.Names ?? [],
            IsRequired = propertyInfo.GetCustomAttribute<RequiredAttribute>(false) is not null,
            Requires =
                propertyInfo.GetCustomAttribute<RequiresAttribute>(false)?.PropertyNames ?? [],
            TerminatingFlag = GetTerminatingFlag(propertyInfo),
        };
}

internal class HelpMetadata
{
    internal required string MetaVarName { get; init; }
    internal required string Help { get; init; }

    internal static HelpMetadata FromPropertyInfo(PropertyInfo propertyInfo) =>
        new()
        {
            MetaVarName =
                propertyInfo.GetCustomAttribute<MetaVarNameAttribute>(false)?.MetaVar
                ?? string.Empty,
            Help =
                propertyInfo.GetCustomAttribute<HelpAttribute>(false)?.Description ?? string.Empty,
        };
}

internal class PropertyMetadata
{
    internal required PropertyInfo Info { get; init; }
    internal required BehaviorMetadata Behavior { get; init; }
    internal required HelpMetadata HelpData { get; init; }
    internal required List<IOptionValidator> Validators { get; init; }

    private static List<IOptionValidator> GetValidators(PropertyInfo propertyInfo) =>
        propertyInfo.GetCustomAttributes(false).OfType<IOptionValidator>().ToList();

    internal bool HasLongOrShortNames() =>
        Behavior.ShortNames.Count > 0 || Behavior.LongNames.Count > 0;

    internal bool IsFlag() =>
        Info.PropertyType == typeof(bool) || Info.PropertyType == typeof(bool?);

    internal static PropertyMetadata FromPropertyInfo(PropertyInfo propertyInfo) =>
        new()
        {
            Info = propertyInfo,
            Behavior = BehaviorMetadata.FromPropertyInfo(propertyInfo),
            HelpData = HelpMetadata.FromPropertyInfo(propertyInfo),
            Validators = GetValidators(propertyInfo),
        };
}

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
