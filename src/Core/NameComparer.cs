namespace NameSorter.Core;

/// <summary>
///     Orders names by last name, then by each given name in turn.
///     Comparison is case-insensitive and uses the invariant culture.
/// </summary>
public sealed class NameComparer : IComparer<Name>
{
    public int Compare(Name? x, Name? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        int byLastName = StringComparer.InvariantCultureIgnoreCase.Compare(x.LastName, y.LastName);
        return byLastName != 0 ? byLastName : CompareGivenNames(x.GivenNames, y.GivenNames);
    }

    private static int CompareGivenNames(IReadOnlyList<string> x, IReadOnlyList<string> y)
    {
        int shared = Math.Min(x.Count, y.Count);
        for (int i = 0; i < shared; i++)
        {
            int result = StringComparer.InvariantCultureIgnoreCase.Compare(x[i], y[i]);
            if (result != 0) return result;
        }

        return x.Count.CompareTo(y.Count);
    }
}
