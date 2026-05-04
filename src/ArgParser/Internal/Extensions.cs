namespace ArgParser.Internal;

internal static class CollectionExtensions
{
    internal static int RemoveFront<T>(this List<T> list, int count)
    {
        int toRemove = Math.Min(count, list.Count);
        if (toRemove <= 0)
            return 0;

        list.Reverse();
        for (int i = 0; i < toRemove; i++)
            list.RemoveAt(list.Count - 1);
        list.Reverse();

        return toRemove;
    }

    internal static IEnumerable<(TSelf, TOther?)> SafeZip<TSelf, TOther>(
        this IEnumerable<TSelf> self,
        IEnumerable<TOther> other
    )
    {
        IEnumerator<TOther> enumerator = other.GetEnumerator();

        foreach (TSelf item in self)
            yield return enumerator.MoveNext() ? (item, enumerator.Current) : (item, default);

        enumerator.Dispose();
    }
}
