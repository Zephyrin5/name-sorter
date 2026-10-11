using NameSorter.Cli;
using NameSorter.Core;
namespace NameSorter.Tests.Helpers;

/// <summary>
/// Records received names, write count, and optionally invocation order.
/// </summary>
internal sealed class RecordingSink(string label = "", List<string>? calls = null) : INameSink
{
    public int WriteCount { get; private set; }

    public IReadOnlyList<string> Received { get; private set; } = [];

    public void Write(IReadOnlyList<Name> names)
    {
        WriteCount++;
        Received = [.. names.Select(name => name.ToString())];
        calls?.Add(label);
    }
}
