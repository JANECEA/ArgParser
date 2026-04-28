using System.Text;
using ArgParser.Internal.Metadata;

namespace ArgParser.Internal;

internal static class StringBuilderExtensions
{
    internal static void AppendWithIndent(this StringBuilder sb, int indent, string line)
    {
        for (int i = 0; i < indent; ++i)
            sb.Append(' ');
        sb.Append(line);
    }

    internal static void AppendLineWithIndent(this StringBuilder sb, int indent, string line)
    {
        sb.AppendWithIndent(indent, line);
        sb.AppendLine();
    }
}

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

        sb.AppendLine("Options:");
        foreach (PropertyMetadata flag in classMetadata.Properties.Where(p => p.IsFlag()))
        {
            sb.AppendLineWithIndent(IndentWidth, GetNameList(flag));
            if (!string.IsNullOrWhiteSpace(flag.HelpData.Help))
                sb.AppendLineWithIndent(IndentWidth * 3, flag.HelpData.Help);
            sb.AppendLine();
        }

        foreach (PropertyMetadata option in classMetadata.Properties.Where(p => !p.IsFlag()))
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
            allNames.Add($"-{shortName}");

        foreach (string longName in property.Behavior.LongNames)
            allNames.Add($"--{longName}");

        return string.Join(", ", allNames);
    }
}
