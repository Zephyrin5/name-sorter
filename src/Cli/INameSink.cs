using NameSorter.Core;

namespace NameSorter.Cli;

/// <summary>A destination for the sorted names.</summary>
internal interface INameSink
{
    void Write(IReadOnlyList<Name> names);
}
