using ArgParser.Attributes;
using ArgParser.Exceptions;
using System.Reflection;

namespace ArgParser;

internal static class AttributeUsageValidator
{
    internal static bool HasLongOrShortNames(PropertyAttributeInfo propertyInfo) =>
        propertyInfo.Functional.ShortNames is not null
        || propertyInfo.Functional.LongNames is not null;

    internal static void ValidateIndividually(PropertyAttributeInfo propertyInfo)
    {
        Type type = propertyInfo.Info.PropertyType;

        bool isFlag =
            type == typeof(bool)
            || type == typeof(bool?);

        if (propertyInfo.Functional.IsRequired && isFlag)
            throw new RequiredOnFlagException(
                $"Property '{propertyInfo.Info.Name}' cannot be marked with [Required] because flags (bool or bool?) are optional by design."
            );

        if (propertyInfo.Functional.TerminatingFlag is not null && !isFlag)
            throw new TerminatingNotOnFlagException(
                $"Property '{propertyInfo.Info.Name}' has [TerminatingFlag], but its type is '{type.Name}'. [TerminatingFlag] can only be used on bool properties."
            );
        CheckIsParsable(type);

        CheckValidatorsMatch(propertyInfo);

    }

    private static void CheckValidatorsMatch(PropertyAttributeInfo propertyInfo)
    {
        Type propertyType = propertyInfo.Info.PropertyType;

        foreach (var validator in propertyInfo.Validators)
        {
            if(!validator.ValidatorType.IsAssignableFrom(propertyType))
            {
                throw new WrongValidatorTypeException("");
            }
        }
    }

    private static void CheckIsParsable(Type type)
    {
        foreach (Type it in type.GetInterfaces().Where(i => i.IsGenericType))
        {
            if (it.GetGenericTypeDefinition() == typeof(IParsable<>) && it.GenericTypeArguments[0] == type)
            {
                return;
            }
        }
        throw new PropertyNotParsableException("");
    }

    internal static void ValidateInfos(List<PropertyAttributeInfo> infos) {
    
        HashSet<string>optionNames = new HashSet<string>();
        foreach (PropertyAttributeInfo info in infos) {
            foreach (var name in info.Functional.ShortNames) { 

            }
        }
    }
}
