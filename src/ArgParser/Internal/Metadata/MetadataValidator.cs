using ArgParser.Attributes;
using ArgParser.Exceptions;

namespace ArgParser.Internal.Metadata;

internal static class MetadataValidator
{
    internal static void Validate(ArgsClassMetadata metadata)
    {
        foreach (PropertyMetadata m in metadata.Properties)
            ValidateIndividually(m);

        CheckForDuplicateShortNames(metadata.Properties);
        CheckForDuplicateLongNames(metadata.Properties);
        ValidateRequiresUsage(metadata.Properties);
        CheckClassValidatorsMatch(metadata);
        ValidatePositional(metadata);
    }

    private static void ValidatePositional(ArgsClassMetadata metadata)
    {
        CheckUnique(metadata);

        Dictionary<string, PropertyMetadata> properties = metadata.Properties.ToDictionary(
            p => p.Info.Name,
            p => p
        );
        foreach (string positionalArg in metadata.PositionalArgs)
        {
            if (!properties.TryGetValue(positionalArg, out PropertyMetadata? property))
                throw new PositionalArgsConfigException("");

            CheckAttributes(property);
        }
    }

    private static void CheckUnique(ArgsClassMetadata metadata)
    {
        HashSet<string> unique = new(metadata.PositionalArgs.Count);
        foreach (string positionalArg in metadata.PositionalArgs)
            if (!unique.Add(positionalArg))
                throw new PositionalArgsConfigException("");
    }

    private static void CheckAttributes(PropertyMetadata property)
    {
        if (property.Behavior.LongNames.Count > 0)
            throw new PositionalArgsConfigException("");

        if (property.Behavior.ShortNames.Count > 0)
            throw new PositionalArgsConfigException("");

        if (property.Behavior.TerminatingFlag is not null)
            throw new PositionalArgsConfigException("");
    }

    private static void ValidateIndividually(PropertyMetadata property)
    {
        Type type = property.Info.PropertyType;
        bool isFlag = type == typeof(bool) || type == typeof(bool?);

        if (property.Behavior.IsRequired && isFlag)
            throw new RequiredOnFlagException(
                $"Property '{property.Info.Name}' cannot be marked with {typeof(RequiredAttribute)} "
                    + "because flags (bool) are optional by design."
            );

        if (property.Behavior.TerminatingFlag is not null && !isFlag)
            throw new TerminatingNotOnFlagException(
                $"Property '{property.Info.Name}' has {typeof(TerminatingFlagAttribute<>)}, "
                    + $"but its type is '{type.Name}'. "
                    + $"{typeof(TerminatingFlagAttribute<>)} can only be used on bool properties."
            );

        CheckIsParsable(property);
        CheckOptionValidatorsMatch(property);
    }

    private static void CheckOptionValidatorsMatch(PropertyMetadata property)
    {
        Type propertyType = property.Info.PropertyType;
        if (Nullable.GetUnderlyingType(propertyType) is Type type)
            propertyType = type;

        foreach (IOptionValidator validator in property.Validators)
        {
            if (!validator.ValidatorType.IsAssignableFrom(propertyType))
                throw new WrongValidatorTypeException(
                    $"Validator '{validator.GetType().Name}' on property '{property.Info.Name}' "
                        + $"expects type '{validator.ValidatorType.Name}' or derived, "
                        + $"but the property type is '{propertyType.Name}'."
                );
        }
    }

    private static void CheckIsParsable(PropertyMetadata property)
    {
        Type propertyType = property.Info.PropertyType;
        propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (propertyType.IsEnum)
            return;

        foreach (Type ifc in propertyType.GetInterfaces().Where(i => i.IsGenericType))
        {
            if (
                ifc.GetGenericTypeDefinition() == typeof(IParsable<>)
                && ifc.GenericTypeArguments[0] == propertyType
            )
                return;
        }
        throw new PropertyNotParsableException(
            $"Type '{propertyType.Name}' on property '{property.Info.Name}' "
                + "is not a valid option type. "
                + $"It must implement IParsable<{propertyType.Name}>."
        );
    }

    private static void CheckClassValidatorsMatch(ArgsClassMetadata classMetadata)
    {
        Type classType = classMetadata.ClassType;

        foreach (IClassValidator validator in classMetadata.Validators)
        {
            if (!validator.ValidatorType.IsAssignableFrom(classType))
                throw new WrongValidatorTypeException(
                    $"Class validator '{validator.GetType().Name}' "
                        + $"expects type '{validator.ValidatorType.Name}' or derived, "
                        + $"but it was applied to '{classType.Name}'."
                );
        }
    }

    private static void ValidateRequiresUsage(IReadOnlyList<PropertyMetadata> metadata)
    {
        HashSet<string> propertyNames = metadata.Select(m => m.Info.Name).ToHashSet();

        foreach (PropertyMetadata property in metadata)
        foreach (string requiredName in property.Behavior.Requires)
        {
            if (!propertyNames.Contains(requiredName))
                throw new ReferencedOptionNotFoundException(
                    $"Property '{property.Info.Name}' uses [Requires(nameof({requiredName}))], "
                        + $"but no option property named '{requiredName}' was found."
                );
        }
    }

    private static void CheckForDuplicateLongNames(IReadOnlyList<PropertyMetadata> metadata)
    {
        Dictionary<string, string> longNames = new();

        foreach (PropertyMetadata property in metadata)
        foreach (string name in property.Behavior.LongNames)
        {
            if (longNames.TryGetValue(name, out string? firstProperty))
                throw new DuplicateOptionNameException(
                    $"Duplicate long option name '--{name}' found on properties "
                        + $"'{firstProperty}' and '{property.Info.Name}'."
                );

            longNames[name] = property.Info.Name;
        }
    }

    private static void CheckForDuplicateShortNames(IReadOnlyList<PropertyMetadata> metadata)
    {
        Dictionary<char, string> shortNames = new();

        foreach (PropertyMetadata property in metadata)
        foreach (char name in property.Behavior.ShortNames)
        {
            if (shortNames.TryGetValue(name, out string? firstProperty))
                throw new DuplicateOptionNameException(
                    $"Duplicate short option name '-{name}' found on properties "
                        + $"'{firstProperty}' and '{property.Info.Name}'."
                );

            shortNames[name] = property.Info.Name;
        }
    }
}
