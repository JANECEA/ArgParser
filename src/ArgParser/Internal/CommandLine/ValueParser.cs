using System.Diagnostics;
using System.Reflection;
using ArgParser.Attributes;
using ArgParser.Exceptions;
using ArgParser.Internal.Metadata;

namespace ArgParser.Internal.CommandLine;

internal static class ValueParser
{
    private static MethodInfo GetParseMethod(Type parseType)
    {
        MethodInfo? parseMethod = parseType.GetMethod(
            "Parse",
            BindingFlags.Public | BindingFlags.Static,
            [typeof(string), typeof(IFormatProvider)]
        );
        if (parseMethod is null)
            throw new UnreachableException("Internal error: Type does not have method Parse");

        return parseMethod;
    }

    private static object ParseEnum(Type parseType, string strValue, EnumCase enumCase)
    {
        if (long.TryParse(strValue, out long _))
            throw new ValueParsingException(
                $"Could not parse value '{strValue}' into type {parseType.Name}. "
                    + "Enum values must be specified by name, not by numeric value."
            );

        switch (enumCase)
        {
            case EnumCase.PreserveCase:
                return Enum.Parse(parseType, strValue, false);

            case EnumCase.AllLowerCase:
                if (strValue.Any(char.IsUpper))
                    throw new ValueParsingException(
                        $"Could not parse value '{strValue}' into type {parseType.Name}. "
                            + "EnumCase.AllLowerCase requires the input to contain only lowercase letters."
                    );

                return Enum.Parse(parseType, strValue, true);

            case EnumCase.AllUpperCase:
                if (strValue.Any(char.IsLower))
                    throw new ValueParsingException(
                        $"Could not parse value '{strValue}' into type {parseType.Name}. "
                            + "EnumCase.AllUpperCase requires the input to contain only uppercase letters."
                    );

                return Enum.Parse(parseType, strValue, true);

            default:
                throw new UnreachableException();
        }
    }

    private static object? CheckSpecialTypes(
        Type parseType,
        PropertyMetadata metadata,
        string strValue
    )
    {
        if (parseType == typeof(string))
            return strValue;

        if (parseType.IsEnum)
        {
            EnumCase enumCase = metadata.Behavior.EnumCasePolicy ?? EnumCase.PreserveCase;
            try
            {
                return ParseEnum(parseType, strValue, enumCase);
            }
            catch (Exception ex) when (ex is not (ValueParsingException or UnreachableException))
            {
                throw new ValueParsingException(
                    $"Could not parse value '{strValue}' into enum {parseType.Name}."
                );
            }
        }

        return null;
    }

    private static Dictionary<PropertyMetadata, object> ParseOptionValues(
        IEnumerable<(PropertyMetadata, string?)> foundOptions
    )
    {
        Dictionary<PropertyMetadata, object> foundValues = new();

        foreach ((PropertyMetadata property, string? strValue) in foundOptions)
        {
            if (strValue is null)
                throw new UnreachableException(
                    "Missing option values should have been caught by CheckMissingOptionValues."
                );

            Type targetType = property.Info.PropertyType;
            Type parseType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (CheckSpecialTypes(parseType, property, strValue) is object value)
            {
                foundValues[property] = value;
                continue;
            }

            MethodInfo parseMethod = GetParseMethod(parseType);
            try
            {
                object parsedValue = parseMethod.Invoke(null, [strValue, null])!;
                foundValues[property] = parsedValue;
            }
            catch
            {
                throw new ValueParsingException(
                    $"Could not parse value '{strValue}' into type {parseType.Name}."
                );
            }
        }
        return foundValues;
    }

    internal static Dictionary<PropertyMetadata, object> GetFoundValues(CoupledArgs coupled)
    {
        IEnumerable<(PropertyMetadata Property, string?)> allCouples = coupled
            .Couples.Select(t => (t.Item1.Property, t.Item2))
            .Concat(coupled.Arguments);

        Dictionary<PropertyMetadata, object> foundValues = ParseOptionValues(allCouples);
        foreach (ArgOccurrence flag in coupled.Flags)
            foundValues[flag.Property] = true;

        return foundValues;
    }
}
