using ArgParser.Attributes;
using ArgParser.Exceptions;
using ArgParser.Internal.Metadata;
using ArgParser.Internal.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        IReadOnlyList<(ArgOccurrence, string?)> foundOptions
    )
    {
        Dictionary<PropertyMetadata, object> foundValues = new();

        foreach ((ArgOccurrence occurence, string? strValue) in foundOptions)
        {
            if (strValue is null)
                throw new UnreachableException(
                    "Missing option values should have been caught by CheckMissingOptionValues."
                );

            Type targetType = occurence.Property.Info.PropertyType;
            Type parseType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (CheckSpecialTypes(parseType, occurence.Property, strValue) is object value)
            {
                foundValues[occurence.Property] = value;
                continue;
            }

            MethodInfo parseMethod = GetParseMethod(parseType);
            try
            {
                object parsedValue = parseMethod.Invoke(null, [strValue, null])!;
                foundValues[occurence.Property] = parsedValue;
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
        Dictionary<PropertyMetadata, object> foundValues = ParseOptionValues(coupled.Couples);
        foreach (ArgOccurrence flag in coupled.Flags)
            foundValues[flag.Property] = true;

        return foundValues;
    }
}
