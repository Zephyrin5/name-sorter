using NameSorter.Core;
namespace NameSorter.Tests.Unit;

public class NameComparerTests
{
    private readonly NameComparer _comparer = new NameComparer();

    private static Name N(string line)
    {
        return NameParser.TryParse(line, out Name? name, out string? error)
            ? name : throw new FormatException($"'{line}' is not a valid name: {error}");
    }

    [Fact]
    public void Compare_DifferentLastNames_OrdersByLastName()
    {
        Assert.True(_comparer.Compare(N("Zed Adams"), N("Adam Baker")) < 0);
    }

    [Fact]
    public void Compare_SameLastName_OrdersByFirstGivenName()
    {
        Assert.True(_comparer.Compare(N("Ann Lee"), N("Bob Lee")) < 0);
    }

    [Fact]
    public void Compare_SameLastAndFirstGivenName_OrdersBySecondGivenName()
    {
        Assert.True(_comparer.Compare(N("Ann Alice Lee"), N("Ann Bella Lee")) < 0);
    }

    [Fact]
    public void Compare_SameLastAndFirstTwoGivenNames_OrdersByThirdGivenName()
    {
        Assert.True(_comparer.Compare(N("Ann Marie Alice Lee"), N("Ann Marie Bella Lee")) < 0);
    }

    [Fact]
    public void Compare_DifferentCase_AreEqual()
    {
        Assert.Equal(0, _comparer.Compare(N("ann LEE"), N("Ann Lee")));
    }

    [Fact]
    public void Compare_AccentedLastName_UsesInvariantCulture()
    {
        Assert.True(_comparer.Compare(N("Ann Äbel"), N("Ann Zeller")) < 0);
    }

    [Fact]
    public void Compare_FewerGivenNamesWithMatchingPrefix_SortsFirst()
    {
        Assert.True(_comparer.Compare(N("Ann Lee"), N("Ann Marie Lee")) < 0);
    }

    [Fact]
    public void Compare_ReversingArguments_ReversesSign()
    {
        Name a = N("Ann Lee");
        Name b = N("Bob Lee");

        Assert.Equal(-Math.Sign(_comparer.Compare(a, b)), Math.Sign(_comparer.Compare(b, a)));
    }

    [Fact]
    public void Compare_IdenticalNames_ReturnsZero()
    {
        Name name = N("Ann Lee");

        Assert.Equal(0, _comparer.Compare(name, name));
        Assert.Equal(0, _comparer.Compare(name, N("Ann Lee")));
    }

    [Fact]
    public void Compare_Nulls_SortBeforeNames()
    {
        Assert.Equal(0, _comparer.Compare(null, null));
        Assert.True(_comparer.Compare(null, N("Ann Lee")) < 0);
        Assert.True(_comparer.Compare(N("Ann Lee"), null) > 0);
    }
}
