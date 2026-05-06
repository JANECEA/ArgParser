using System.Text;
using ArgParser.Internal.Metadata;

namespace ArgParser.Internal;

internal static class HelpMessageGenerator
{
    private const int IndentWidth = 4;

    internal static string Generate(ArgsClassMetadata classMetadata)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(classMetadata.ExampleUsage))
        {
            sb.AppendLine(classMetadata.ExampleUsage);
            sb.AppendLine();
        }

        if(classMetadata.Arguments.Count != 0)
        {
            sb.AppendLine("Arguments:");
        }
        foreach (PropertyMetadata argument in classMetadata.Arguments)
        {
            sb.AppendWithIndent(IndentWidth, "");
            sb.AppendLine($"{GetMetaVar(argument)}");
            if (!string.IsNullOrWhiteSpace(argument.HelpData.Help))
                sb.AppendLineWithIndent(IndentWidth * 3, argument.HelpData.Help);
            sb.AppendLine();
        }

        if (classMetadata.Options.Count != 0)
        {
            sb.AppendLine("Options:");
        }
        foreach (PropertyMetadata flag in classMetadata.Options.Where(p => p.IsFlag()))
        {
            sb.AppendLineWithIndent(IndentWidth, GetNameList(flag));
            if (!string.IsNullOrWhiteSpace(flag.HelpData.Help))
                sb.AppendLineWithIndent(IndentWidth * 3, flag.HelpData.Help);
            sb.AppendLine();
        }

        foreach (PropertyMetadata option in classMetadata.Options.Where(p => !p.IsFlag()))
        {
            sb.AppendWithIndent(IndentWidth, GetNameList(option));
            sb.AppendLine($" {GetMetaVar(option)}");
            if (!string.IsNullOrWhiteSpace(option.HelpData.Help))
                sb.AppendLineWithIndent(IndentWidth * 3, option.HelpData.Help);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GetMetaVar(PropertyMetadata property) =>
        string.IsNullOrWhiteSpace(property.HelpData.MetaVarName)
            ? property.Info.Name
            : property.HelpData.MetaVarName;

    private static string GetNameList(PropertyMetadata property)
    {
        List<string> allNames = new();
        foreach (char shortName in property.Behavior.ShortNames)
            allNames.Add(CliStandards.GetShortName(shortName));

        foreach (string longName in property.Behavior.LongNames)
            allNames.Add(CliStandards.GetLongName(longName));

        return string.Join(", ", allNames);
    }
}
