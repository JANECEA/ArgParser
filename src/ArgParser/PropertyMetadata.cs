using System.Reflection;
using ArgParser.Attributes;

namespace ArgParser;

internal class BehaviorMetadata
{
    internal required List<char> ShortNames { get; init; }
    internal required List<string> LongNames { get; init; }
    internal bool IsRequired { get; init; }
    internal required List<string> Requires { get; init; }
    internal ITerminatingFlag? TerminatingFlag { get; init; }

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

    internal static PropertyMetadata FromPropertyInfo(PropertyInfo propertyInfo) =>
        new()
        {
            Info = propertyInfo,
            Behavior = BehaviorMetadata.FromPropertyInfo(propertyInfo),
            HelpData = HelpMetadata.FromPropertyInfo(propertyInfo),
            Validators = GetValidators(propertyInfo),
        };
}
