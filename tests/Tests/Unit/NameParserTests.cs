using NameSorter.Core;

namespace NameSorter.Tests.Unit;

public class NameParserTests
{
    [Theory]
    [InlineData("Ann Lee", new[] { "Ann" })]
    [InlineData("Ann Marie Lee", new[] { "Ann", "Marie" })]
    [InlineData("Ann Marie Louise Lee", new[] { "Ann", "Marie", "Louise" })]
    public void TryParse_OneToThreeGivenNames_SplitsGivenNamesFromLastName(
        string line, string[] expectedGivenNames)
    {
        bool success = NameParser.TryParse(line, out Name? name, out string? error);

        Assert.True(success, error);
        Assert.NotNull(name);
        Assert.Null(error);
        Assert.Equal(expectedGivenNames, name.GivenNames);
        Assert.Equal("Lee", name.LastName);
    }

    [Fact]
    public void TryParse_ExtraWhitespaceAndTabs_NormalizesWhitespace()
    {
        bool success = NameParser.TryParse(
            "  Ann \t Marie   Lee  ", out Name? name, out string? error);

        Assert.True(success, error);
        Assert.NotNull(name);
        Assert.Equal("Ann Marie Lee", name.ToString());
    }

    [Fact]
    public void TryParse_MixedCase_PreservesOriginalCasing()
    {
        bool success = NameParser.TryParse(
            "ann McDonald", out Name? name, out string? error);

        Assert.True(success, error);
        Assert.NotNull(name);
        Assert.Equal("ann McDonald", name.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void TryParse_NullOrWhitespace_ReturnsFalseWithError(string? line)
    {
        bool success = NameParser.TryParse(line, out Name? name, out string? error);

        Assert.False(success);
        Assert.Null(name);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }

    [Theory]
    [InlineData("Madonna")]
    [InlineData("A B C D Lee")]
    public void TryParse_InvalidNamePartCount_ReturnsFalseWithError(string line)
    {
        bool success = NameParser.TryParse(line, out Name? name, out string? error);

        Assert.False(success);
        Assert.Null(name);
        Assert.False(string.IsNullOrWhiteSpace(error));
    }
}
