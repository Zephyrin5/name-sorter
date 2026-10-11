using NameSorter.Cli;
using NameSorter.Tests.Helpers;
using NameSorter.Tests.Helpers.NameSinks;

namespace NameSorter.Tests.Unit;

public class NameSortingJobTests
{
    [Fact]
    public void Run_BriefExample_WritesSortedNamesToSinkInOneCall()
    {
        RecordingSink sink = new RecordingSink();

        new NameSortingJob([sink]).Run(BriefExample.Input);

        Assert.Equal(1, sink.WriteCount);
        Assert.Equal(BriefExample.Expected, sink.Received);
    }

    [Fact]
    public void Run_MultipleSinks_EachReceivesTheSameSortedNames()
    {
        RecordingSink first = new RecordingSink();
        RecordingSink second = new RecordingSink();

        new NameSortingJob([first, second]).Run(["Bob Lee", "Ann Lee"]);

        Assert.Equal(["Ann Lee", "Bob Lee"], first.Received);
        Assert.Equal(["Ann Lee", "Bob Lee"], second.Received);
    }

    [Fact]
    public void Run_MultipleSinks_WritesInTheOrderGiven()
    {
        List<string> calls = [];
        RecordingSink first = new RecordingSink("first", calls);
        RecordingSink second = new RecordingSink("second", calls);

        new NameSortingJob([first, second]).Run(["Ann Lee"]);

        Assert.Equal(["first", "second"], calls);
    }

    [Theory]
    [InlineData("Madonna")]
    [InlineData("A B C D Lee")]
    public void Run_InvalidLine_ThrowsAndWritesNothing(string invalidName)
    {
        RecordingSink sink = new RecordingSink();

        FormatException exception = Assert.Throws<FormatException>(() => new NameSortingJob([sink]).Run(["Ann Lee", invalidName]));

        Assert.StartsWith("Line 2:", exception.Message);
        Assert.Equal(0, sink.WriteCount);
    }

    [Fact]
    public void Run_NoNames_WritesEmptyListToSinks()
    {
        RecordingSink sink = new RecordingSink();

        new NameSortingJob([sink]).Run(["", "   "]);

        Assert.Equal(1, sink.WriteCount);
        Assert.Empty(sink.Received);
    }

    [Fact]
    public void Run_EqualSortKeys_PreservesInputOrderAndDuplicates()
    {
        RecordingSink sink = new RecordingSink();
        string[] input = ["ann lee", "Ann Lee", "ANN LEE", "Ann Lee"];

        new NameSortingJob([sink]).Run(input);

        Assert.Equal(input, sink.Received);
    }

    [Fact]
    public void Run_NullLines_ThrowsArgumentNullException()
    {
        RecordingSink sink = new RecordingSink();

        Assert.Throws<ArgumentNullException>(() => new NameSortingJob([sink]).Run(null!));

        Assert.Equal(0, sink.WriteCount);
    }
}
