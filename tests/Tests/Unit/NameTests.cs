using NameSorter.Core;

namespace NameSorter.Tests.Unit;

public class NameTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Constructor_OneToThreeGivenNames_Succeeds(int count)
    {
        string[] given = [.. Enumerable.Range(1, count).Select(i => $"G{i}")];

        Name name = new Name(given, "Smith");

        Assert.Equal(given, name.GivenNames);
    }

    [Fact]
    public void Constructor_NoGivenNames_Throws()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Name([], "Smith"));
        Assert.Equal("givenNames", exception.ParamName);
    }

    [Fact]
    public void Constructor_FourGivenNames_Throws()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Name(["A", "B", "C", "D"], "Smith"));

        Assert.Equal("givenNames", exception.ParamName);
    }

    [Fact]
    public void Constructor_NullGivenNames_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new Name(null!, "Smith"));

        Assert.Equal("givenNames", exception.ParamName);
    }

    [Fact]
    public void Constructor_NullLastName_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new Name(["Ann"], null!));

        Assert.Equal("lastName", exception.ParamName);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_BlankGivenName_Throws(string? given)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Name(["Ann", given!], "Smith"));

        Assert.Equal("givenNames", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankLastName_Throws(string lastName)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Name(["Ann"], lastName));

        Assert.Equal("lastName", exception.ParamName);
    }

    [Theory]
    [InlineData("Ann Marie")]
    [InlineData("Ann\tMarie")]
    [InlineData(" Ann")]
    [InlineData("Ann ")]
    public void Constructor_GivenNameContainsWhitespace_Throws(string givenName)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Name([givenName], "Smith"));

        Assert.Equal("givenNames", exception.ParamName);
    }

    [Theory]
    [InlineData("Van Dyke")]
    [InlineData("Van\tDyke")]
    [InlineData(" Smith")]
    [InlineData("Smith ")]
    public void Constructor_LastNameContainsWhitespace_Throws(string lastName)
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Name(["Ann"], lastName));

        Assert.Equal("lastName", exception.ParamName);
    }

    [Fact]
    public void Constructor_CopiesInput_SoLaterChangesDoNotAffectName()
    {
        var givenNamesInput = new List<string> { "Ann" };
        Name name = new Name(givenNamesInput, "Smith");

        givenNamesInput[0] = "Bob";
        givenNamesInput.Add("Extra");

        Assert.Equal("Ann Smith", name.ToString());
    }

    [Fact]
    public void ToString_JoinsGivenNamesThenLastName()
    {
        Assert.Equal("Ann Marie Smith", new Name(["Ann", "Marie"], "Smith").ToString());
    }
}
