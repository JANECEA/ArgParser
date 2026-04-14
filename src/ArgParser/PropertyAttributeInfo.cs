using System.Reflection;
using ArgParser.Attributes;

namespace ArgParser;

internal class FunctionalAttributes
{
    internal ShortNamesAttribute? ShortNames { get; init; }
    internal LongNamesAttribute? LongNames { get; init; }
    internal bool IsRequired { get; init; }
    internal RequiresAttribute? Requires { get; init; }
    internal ITerminatingFlag? TerminatingFlag { get; init; }

    private static ITerminatingFlag? GetTerminatingFlag(PropertyInfo propertyInfo) =>
        propertyInfo.GetCustomAttributes(false).OfType<ITerminatingFlag>().FirstOrDefault();

    internal static FunctionalAttributes FromPropertyInfo(PropertyInfo propertyInfo) =>
        new()
        {
            ShortNames = propertyInfo.GetCustomAttribute<ShortNamesAttribute>(false),
            LongNames = propertyInfo.GetCustomAttribute<LongNamesAttribute>(false),
            IsRequired = propertyInfo.GetCustomAttribute<RequiredAttribute>(false) is not null,
            Requires = propertyInfo.GetCustomAttribute<RequiresAttribute>(false),
            TerminatingFlag = GetTerminatingFlag(propertyInfo),
        };
}

internal class InformationalAttributes
{
    internal MetaVarNameAttribute? MetaVarName { get; init; }
    internal HelpAttribute? Help { get; init; }

    internal static InformationalAttributes FromPropertyInfo(PropertyInfo propertyInfo) =>
        new()
        {
            MetaVarName = propertyInfo.GetCustomAttribute<MetaVarNameAttribute>(false),
            Help = propertyInfo.GetCustomAttribute<HelpAttribute>(false),
        };
}

internal class PropertyAttributeInfo
{
    internal required PropertyInfo Info { get; init; }
    internal required FunctionalAttributes Functional { get; init; }
    internal required InformationalAttributes Informational { get; init; }
    internal required List<IOptionValidator> Validators { get; init; }

    private static List<IOptionValidator> GetValidators(PropertyInfo propertyInfo) =>
        propertyInfo.GetCustomAttributes(false).OfType<IOptionValidator>().ToList();

    internal static PropertyAttributeInfo FromPropertyInfo(PropertyInfo propertyInfo) =>
        new()
        {
            Info = propertyInfo,
            Functional = FunctionalAttributes.FromPropertyInfo(propertyInfo),
            Informational = InformationalAttributes.FromPropertyInfo(propertyInfo),
            Validators = GetValidators(propertyInfo),
        };
}
