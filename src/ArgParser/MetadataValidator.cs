using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace ArgParser;

internal static class MetadataValidator
{
    internal static bool HasLongOrShortNames(PropertyMetadata metadata) =>
        metadata.Behavior.ShortNames.Count > 0 || metadata.Behavior.LongNames.Count > 0;

    private static void ValidateIndividually(PropertyMetadata metadata)
    {
        Type type = metadata.Info.PropertyType;
        bool isFlag = type == typeof(bool) || type == typeof(bool?);

        if (metadata.Behavior.IsRequired && isFlag)
            throw new RequiredOnFlagException(
                $"Property '{metadata.Info.Name}' cannot be marked with [Required] because flags (bool or bool?) are optional by design."
            );

        if (metadata.Behavior.TerminatingFlag is not null && !isFlag)
            throw new TerminatingNotOnFlagException(
                $"Property '{metadata.Info.Name}' has [TerminatingFlag], but its type is '{type.Name}'. [TerminatingFlag] can only be used on bool properties."
            );

        CheckIsParsable(type);
        CheckValidatorsMatch(metadata);
    }

    private static void CheckValidatorsMatch(PropertyMetadata metadata)
    {
        Type propertyType = metadata.Info.PropertyType;

        foreach (IOptionValidator validator in metadata.Validators)
        {
            if (!validator.ValidatorType.IsAssignableFrom(propertyType))
                throw new WrongValidatorTypeException(
                    $"Validator '{validator.GetType().Name}' on property '{metadata.Info.Name}' expects type '{validator.ValidatorType.Name}', but the property type is '{propertyType.Name}'."
                );
        }
    }

    private static void CheckIsParsable(Type propertyType)
    {
        foreach (Type ifc in propertyType.GetInterfaces().Where(i => i.IsGenericType))
        {
            if (
                ifc.GetGenericTypeDefinition() == typeof(IParsable<>)
                && ifc.GenericTypeArguments[0] == propertyType
            )
                return;
        }
        throw new PropertyNotParsableException(
            $"Type '{propertyType.Name}' is not a valid option type. It must implement IParsable<{propertyType.Name}>."
        );
    }

    internal static void Validate(List<PropertyMetadata> metadata)
    {
        foreach (PropertyMetadata m in metadata)
            ValidateIndividually(m);

        HashSet<string> optionNames = new HashSet<string>();
        foreach (PropertyMetadata info in metadata)
        {
            foreach (var name in info.Behavior.ShortNames) { }
        }
    }
}
