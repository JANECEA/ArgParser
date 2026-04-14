using ArgParser.Exceptions;

namespace ArgParser;

internal static class AttributeUsageValidator
{
    internal static bool HasLongOrShortNames(PropertyAttributeInfo propertyInfo) =>
        propertyInfo.Functional.ShortNames is not null
        || propertyInfo.Functional.LongNames is not null;

    internal static void ValidateIndividually(PropertyAttributeInfo propertyInfo)
    {
        bool isFlag =
            propertyInfo.Info.PropertyType == typeof(bool)
            || propertyInfo.Info.PropertyType == typeof(bool?);

        if (propertyInfo.Functional.IsRequired && isFlag)
            throw new RequiredOnFlagException(
                $"Property '{propertyInfo.Info.Name}' cannot be marked with [Required] because flags (bool or bool?) are optional by design."
            );

        if (propertyInfo.Functional.TerminatingFlag is not null && !isFlag)
            throw new TerminatingNotOnFlagException(
                $"Property '{propertyInfo.Info.Name}' has [TerminatingFlag], but its type is '{propertyInfo.Info.PropertyType.Name}'. [TerminatingFlag] can only be used on bool properties."
            );
    }

    internal static void ValidateInfos(List<PropertyAttributeInfo> infos) { }
}
