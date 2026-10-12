using NameSorter.Core;

namespace NameSorter.Tests.Unit;

public class NameComparerTests
{
    private readonly NameComparer _comparer = new NameComparer();

    private static Name NameOf(string line)
    {
        return NameParser.TryParse(line, out Name? name, out string? error)
            ? name
            : throw new FormatException($"'{line}' is not a valid name: {error}");
    }

    [Fact]
    public void Compare_DifferentLastNames_OrdersByLastName()
    {
        Name a = NameOf("Zed Adams");
        Name b = NameOf("Adam Baker");

        Assert.True(_comparer.Compare(a, b) < 0);
        Assert.True(_comparer.Compare(b, a) > 0);
    }

    [Fact]
    public void Compare_SameLastName_OrdersByFirstGivenName()
    {
        Assert.True(_comparer.Compare(NameOf("Ann Lee"), NameOf("Bob Lee")) < 0);
    }

    [Fact]
    public void Compare_SameLastAndFirstGivenName_OrdersBySecondGivenName()
    {
        Assert.True(_comparer.Compare(NameOf("Ann Alice Lee"), NameOf("Ann Bella Lee")) < 0);
    }

    [Fact]
    public void Compare_SameLastAndFirstTwoGivenNames_OrdersByThirdGivenName()
    {
        Assert.True(_comparer.Compare(NameOf("Ann Marie Alice Lee"), NameOf("Ann Marie Bella Lee")) < 0);
    }

    [Fact]
    public void Compare_DifferentCase_AreEqual()
    {
        Assert.Equal(0, _comparer.Compare(NameOf("ann LEE"), NameOf("Ann Lee")));
    }

    [Fact]
    public void Compare_AccentedLastName_SortsAfterAsciiLetters()
    {
        Assert.True(_comparer.Compare(NameOf("Ann Äbel"), NameOf("Ann Zeller")) > 0);
    }

    [Fact]
    public void Compare_FewerGivenNamesWithMatchingPrefix_SortsFirst()
    {
        Assert.True(_comparer.Compare(NameOf("Ann Lee"), NameOf("Ann Marie Lee")) < 0);
    }

    [Fact]
    public void Compare_IdenticalNames_ReturnsZero()
    {
        Name name = NameOf("Ann Lee");

        Assert.Equal(0, _comparer.Compare(name, name));
        Assert.Equal(0, _comparer.Compare(name, NameOf("Ann Lee")));
    }

    [Fact]
    public void Compare_Nulls_SortBeforeNames()
    {
        Assert.Equal(0, _comparer.Compare(null, null));
        Assert.True(_comparer.Compare(null, NameOf("Ann Lee")) < 0);
        Assert.True(_comparer.Compare(NameOf("Ann Lee"), null) > 0);
    }
}
