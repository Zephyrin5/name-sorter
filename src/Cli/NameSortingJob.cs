using NameSorter.Core;
namespace NameSorter.Cli;

internal sealed class NameSortingJob(IReadOnlyList<INameSink> sinks)
{
    public void Run(IEnumerable<string> lines)
    {
        IReadOnlyList<Name> sorted =
        [
            .. NameListParser
                .ParseLines(lines)
                .OrderBy(name => name, new NameComparer())
        ];

        foreach (INameSink sink in sinks)
            sink.Write(sorted);
    }
}
