using NameSorter.Cli;
using NameSorter.Core;
using NameSorter.Tests.Helpers;

namespace NameSorter.Tests.Unit;

public class NameSortingJobTests
{
    [Fact]
    public void Run_BriefExample_WritesSortedNamesToSinkInOneCall()
    {
        RecordingSink sink = new RecordingSink();

        CreateJob(sink).Run(BriefExample.Input);

        Assert.Equal(1, sink.WriteCount);
        Assert.Equal(BriefExample.Expected, sink.Received);
    }

    [Fact]
    public void Run_MultipleSinks_EachReceivesTheSameSortedNames()
    {
        RecordingSink first = new RecordingSink();
        RecordingSink second = new RecordingSink();

        CreateJob(first, second).Run(["Bob Lee", "Ann Lee"]);

        Assert.Equal(["Ann Lee", "Bob Lee"], first.Received);
        Assert.Equal(["Ann Lee", "Bob Lee"], second.Received);
    }

    [Fact]
    public void Run_MultipleSinks_WritesInTheOrderGiven()
    {
        List<string> calls = [];
        RecordingSink first = new RecordingSink("first", calls);
        RecordingSink second = new RecordingSink("second", calls);

        CreateJob(first, second).Run(["Ann Lee"]);

        Assert.Equal(["first", "second"], calls);
    }

    [Fact]
    public void Run_CustomComparer_UsesProvidedOrdering()
    {
        RecordingSink sink = new RecordingSink();
        IComparer<Name> comparer = Comparer<Name>.Create((x, y) => StringComparer.Ordinal.Compare(y.LastName, x.LastName));

        new NameSortingJob(comparer, [sink]).Run(["Ann Lee", "Bob Smith"]);

        Assert.Equal(["Bob Smith", "Ann Lee"], sink.Received);
    }

    [Theory]
    [InlineData("Madonna")]
    [InlineData("A B C D Lee")]
    public void Run_InvalidLine_ThrowsAndWritesNothing(string invalidName)
    {
        RecordingSink sink = new RecordingSink();

        FormatException exception = Assert.Throws<FormatException>(() => CreateJob(sink).Run(["Ann Lee", invalidName]));

        Assert.StartsWith("Line 2:", exception.Message);
        Assert.Equal(0, sink.WriteCount);
    }

    [Fact]
    public void Run_NoNames_WritesEmptyListToSinks()
    {
        RecordingSink sink = new RecordingSink();

        CreateJob(sink).Run(["", "   "]);

        Assert.Equal(1, sink.WriteCount);
        Assert.Empty(sink.Received);
    }

    [Fact]
    public void Run_NullLines_ThrowsArgumentNullException()
    {
        RecordingSink sink = new RecordingSink();

        Assert.Throws<ArgumentNullException>(() => CreateJob(sink).Run(null!));

        Assert.Equal(0, sink.WriteCount);
    }

    private static NameSortingJob CreateJob(params INameSink[] sinks)
    {
        return new NameSortingJob(new NameComparer(), sinks);
    }
}
