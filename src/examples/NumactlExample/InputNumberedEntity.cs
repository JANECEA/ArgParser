using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NumactlExample;

internal class InputNumberedEntity : IParsable<InputNumberedEntity>
{
    private static readonly string _allText = "all";

    public bool All { get; set; } = false;

    public List<int> Entities { get; set; } = [];

    public static InputNumberedEntity Parse(string s, IFormatProvider? provider)
    {
        var entity = new InputNumberedEntity();
        if (s == _allText)
        {
            entity.All = true;
        }
        else
        {
            //I believe parsing itself is not necessary for this task, hence just a comment
            //parse numbers
        }

        return entity;
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out InputNumberedEntity result
    )
    {
        result = new();
        if (s == _allText)
        {
            result.All = true;
        }
        else
        {
            //I believe parsing itself is not necessary for this task, hence just a comment
            //parse numbers
        }

        return true;
    }

    public override string ToString()
    {
        if (All)
        {
            return _allText;
        }

        var builder = new StringBuilder();
        foreach (var number in Entities)
        {
            builder.Append($"{number}, ");
        }

        return builder.ToString();
    }
}
