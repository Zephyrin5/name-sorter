using NameSorter.Core;

namespace NameSorter.Cli;

/// <summary>Prints names to the console, one per line.</summary>
internal sealed class ConsoleNameSink(TextWriter output) : INameSink
{
    public ConsoleNameSink() : this(Console.Out) { }

    public void Write(IReadOnlyList<Name> names)
    {
        foreach (Name name in names)
            output.WriteLine(name);
    }
}
