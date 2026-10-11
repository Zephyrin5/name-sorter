using NameSorter.Core;

namespace NameSorter.Cli;

/// <summary>
///     Parses lines into names, sorts them, and writes the result to each sink in order.
///     All input is validated before any sink is written to, so invalid input produces no output.
/// </summary>
internal sealed class NameSortingJob(IComparer<Name> comparer, IReadOnlyList<INameSink> sinks)
{
    /// <exception cref="FormatException">A line is not a valid name. Nothing is written.</exception>
    public void Run(IEnumerable<string> lines)
    {
        IReadOnlyList<Name> sorted =
        [
            .. NameListParser
                .ParseLines(lines)
                .OrderBy(name => name, comparer)
        ];

        foreach (INameSink sink in sinks)
            sink.Write(sorted);
    }
}
