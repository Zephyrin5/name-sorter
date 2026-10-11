using NameSorter.Cli;
using NameSorter.Core;

namespace NameSorter.Tests.Unit;

public class ConsoleNameSinkTests
{
    [Fact]
    public void Write_PrintsOneNamePerLineInOrder()
    {
        StringWriter output = new StringWriter();
        Name[] names = [new Name(["Ann"], "Lee"), new Name(["Bob", "Jay"], "Smith")];

        new ConsoleNameSink(output).Write(names);

        Assert.Equal(
            $"Ann Lee{Environment.NewLine}Bob Jay Smith{Environment.NewLine}",
            output.ToString());
    }

    [Fact]
    public void Write_NoNames_PrintsNothing()
    {
        StringWriter output = new StringWriter();

        new ConsoleNameSink(output).Write([]);

        Assert.Equal(string.Empty, output.ToString());
    }
}
