using System.Diagnostics;
using System.Reflection;
using ArgParser.Attributes;
using ArgParser.Exceptions;
using ArgParser.Internal;
using ArgParser.Internal.CommandLine;
using ArgParser.Internal.Metadata;
using ArgParser.Internal.Parsing;

namespace ArgParser;

/// <summary>
/// Implements parsing of standard program command line arguments into the specified type
/// </summary>
/// <typeparam name="TArgs">Declared argument type</typeparam>
public sealed class ArgParser<TArgs>
    where TArgs : BaseArgs, new()
{
    private readonly ProcessedClassMetadata _metadata;
    private readonly Lazy<string> _helpMessage;

    internal ArgParser(ProcessedClassMetadata metadata, Lazy<string> helpMessage)
    {
        _metadata = metadata;
        _helpMessage = helpMessage;
    }

    private static void CheckTerminatingFlags(IReadOnlyList<ArgOccurrence> flags)
    {
        for (int i = flags.Count - 1; i >= 0; i--)
        {
            if (flags[i].Property.Behavior.TerminatingFlag is ITerminatingFlag t)
                t.ThrowException();
        }
    }

    private void ResetFlagValues(TArgs argsObject)
    {
        foreach (PropertyMetadata flag in _metadata.AllFlags)
            flag.Info.SetValue(argsObject, false);
    }

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

    private static void SetAllValues(
        TArgs argsObject,
        Dictionary<PropertyMetadata, object> foundValues
    )
    {
        foreach ((PropertyMetadata property, object value) in foundValues)
            property.Info.SetValue(argsObject, value);
    }

    private static Dictionary<PropertyMetadata, object> GetFoundValues(CoupledArgs coupled)
    {
        Dictionary<PropertyMetadata, object> foundValues = ParseOptionValues(coupled.Couples);
        foreach (ArgOccurrence flag in coupled.Flags)
            foundValues[flag.Property] = true;

        return foundValues;
    }

    private TArgs GetArgObject(
        CoupledArgs coupled,
        Dictionary<PropertyMetadata, object> foundValues
    )
    {
        TArgs argObject = new()
        {
            HelpCalled = false,
            PlainArguments = coupled
                .PlainBeforeDelimiter.Concat(coupled.PlainAfterDelimiter)
                .ToArray(),
        };
        ResetFlagValues(argObject);
        SetAllValues(argObject, foundValues);
        return argObject;
    }

    /// <summary>
    /// Tries parsing the command line arguments according to the structure of
    /// TArgs and attributes defined in it.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <exception cref="CommandLineParsingException">Argument parsing has failed</exception>
    /// <exception cref="HelpCalledException">Program was called with the 'help' terminating flag</exception>
    public TArgs Parse(string[] args)
    {
        CoupledArgs coupled = CoupledArgs.FromArgs(args, _metadata);
        CheckTerminatingFlags(coupled.Flags);

        CommandLineValidator.CheckUnknownArguments(coupled.PlainBeforeDelimiter);
        CommandLineValidator.CheckMissingOptionValues(coupled.Couples);
        CommandLineValidator.CheckDuplicateOccurrences(coupled.Couples, coupled.Flags);

        Dictionary<PropertyMetadata, object> foundValues = GetFoundValues(coupled);

        CommandLineValidator.CheckRequired(foundValues, _metadata);
        CommandLineValidator.CheckRequires(foundValues);

        CommandLineValidator.ApplyOptionValidators(foundValues);

        TArgs argObject = GetArgObject(coupled, foundValues);

        CommandLineValidator.ApplyClassValidators(argObject, _metadata);
        return argObject;
    }

    /// <summary>
    /// Generates a help message based on the information provided in attributes:
    /// <see cref="HelpAttribute"/>,
    /// <br/>
    /// <see cref="MetaVarNameAttribute"/>, and
    /// <br/>
    /// <see cref="ExampleUsageAttribute"/>.
    /// </summary>
    public string GenerateHelpMessage() => _helpMessage.Value;
}
