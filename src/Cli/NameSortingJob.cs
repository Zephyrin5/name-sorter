using NameSorter.Core;
namespace NameSorter.Cli;

internal sealed class NameSortingJob(IComparer<Name> comparer, IReadOnlyList<INameSink> sinks)
{
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
