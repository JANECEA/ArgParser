namespace ArgParser.Internal;

internal static class CliStandards
{
    private static readonly char[] AllowedChars = ['-', '.', ':', '_'];

    internal static char ValueSeparator => '=';

    internal static string Delimiter => "--";

    internal static bool IsValidShortName(char shortName) => char.IsAsciiLetter(shortName);

    internal static bool IsValidLongName(string longName)
    {
        if (string.IsNullOrWhiteSpace(longName))
            return false;

        if (char.IsAsciiDigit(longName[0]) || AllowedChars.Contains(longName[0]))
            return false;

        foreach (char c in longName)
            if (!char.IsAsciiLetterOrDigit(c) && !AllowedChars.Contains(c))
                return false;

        return true;
    }

    internal static bool IsOptionLike(string word) => word.StartsWith('-');

    internal static string GetShortName(char shortName) => $"-{shortName}";

    internal static string GetLongName(string longName) => $"--{longName}";
}
