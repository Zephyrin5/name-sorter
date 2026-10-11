using NameSorter.Cli;
using NameSorter.Core;
namespace NameSorter.Tests.Helpers.NameSinks;

/// <summary>Throws the given exception when written to.</summary>
internal sealed class ThrowingSink(Exception exception) : INameSink
{
    public void Write(IReadOnlyList<Name> names)
    {
        throw exception;
    }
}
