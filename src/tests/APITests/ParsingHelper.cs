namespace Tests.APITests;

public static class ParsingHelper
{
    public static string[] GetSplitArgs(string args)
    {
        return string.IsNullOrWhiteSpace(args)
            ? Array.Empty<string>()
            : args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}
