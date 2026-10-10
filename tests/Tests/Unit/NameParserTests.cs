using NameSorter.Core;

namespace NameSorter.Tests.Unit;

public class NameParserTests
{
    [Theory]
    [InlineData("Ann Lee", new[] { "Ann" })]
    [InlineData("Ann Marie Lee", new[] { "Ann", "Marie" })]
    [InlineData("Ann Marie Louise Lee", new[] { "Ann", "Marie", "Louise" })]
    public void Parse_OneToThreeGivenNames_SplitsGivenNamesFromLastName(
        string line, string[] expectedGivenNames)
    {
        Name name = NameParser.Parse(line);

        Assert.Equal(expectedGivenNames, name.GivenNames);
        Assert.Equal("Lee", name.LastName);
    }

    [Fact]
    public void Parse_ExtraWhitespaceAndTabs_IsTolerated()
    {
        Name name = NameParser.Parse("  Ann \t Marie   Lee  ");

        Assert.Equal("Ann Marie Lee", name.ToString());
    }

    [Fact]
    public void Parse_PreservesOriginalCasing()
    {
        Assert.Equal("ann McDonald", NameParser.Parse("ann McDonald").ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Madonna")]
    public void Parse_FewerThanTwoTokens_ThrowsFormatException(string line)
    {
        Assert.Throws<FormatException>(() => NameParser.Parse(line));
    }

    [Fact]
    public void Parse_FourGivenNames_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => NameParser.Parse("A B C D Lee"));
    }

    [Fact]
    public void Parse_Null_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => NameParser.Parse(null!));

        Assert.Equal("nameString", exception.ParamName);
    }
}
