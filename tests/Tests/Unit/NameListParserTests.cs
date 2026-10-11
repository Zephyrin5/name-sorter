using NameSorter.Cli;

namespace NameSorter.Tests.Unit;

public class NameListParserTests
{
    [Fact]
    public void ParseLines_ValidNames_PreservesInputOrder()
    {
        string[] lines =
        [
            "Bob Lee",
            "Ann Marie Smith",
            "Hunter Uriah Mathew Clarke"
        ];

        var names = NameListParser.ParseLines(lines);

        Assert.Equal(lines, names.Select(name => name.ToString()).ToArray());
    }

    [Fact]
    public void ParseLines_BlankLines_AreSkipped()
    {
        string[] lines = ["", "Bob Lee", "   ", "\t", "Ann Smith", ""];

        var names = NameListParser.ParseLines(lines);

        Assert.Equal(
            ["Bob Lee", "Ann Smith" ],
            names.Select(name => name.ToString()).ToArray());
    }

    [Fact]
    public void ParseLines_EmptyInput_ReturnsEmptyList()
    {
        var names = NameListParser.ParseLines([]);

        Assert.Empty(names);
    }

    [Fact]
    public void ParseLines_OnlyBlankLines_ReturnsEmptyList()
    {
        var names = NameListParser.ParseLines(["", " ", "\t"]);

        Assert.Empty(names);
    }

    [Theory]
    [InlineData("Madonna")]
    [InlineData("A B C D Lee")]
    public void ParseLines_InvalidName_ReportsLineNumberIncludingBlankLines(
        string invalidName)
    {
        string[] lines = ["Ann Lee", "", "   ", invalidName];

        FormatException exception = Assert.Throws<FormatException>(() => NameListParser.ParseLines(lines));

        Assert.StartsWith("Line 4:", exception.Message);
        Assert.Contains(invalidName, exception.Message);
        Assert.IsType<FormatException>(exception.InnerException);
    }

    [Fact]
    public void ParseLines_NullInput_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => NameListParser.ParseLines(null!));

        Assert.Equal("lines", exception.ParamName);
    }
}
