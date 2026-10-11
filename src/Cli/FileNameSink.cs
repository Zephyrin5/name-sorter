using NameSorter.Core;

namespace NameSorter.Cli;

/// <summary>Writes names to a file, one per line, replacing any existing file.</summary>
internal sealed class FileNameSink(string path) : INameSink
{
    public void Write(IReadOnlyList<Name> names)
    {
        string destination = Path.GetFullPath(path);
        string directory = Path.GetDirectoryName(destination) ?? throw new InvalidOperationException($"'{destination}' has no parent directory.");

        // Write to a temporary file in the destination directory before replacing
        // the target, so a failed write doesn't truncate the existing output.
        string tempPath = Path.Combine(
            directory,
            $".{Path.GetFileName(destination)}.{Guid.NewGuid():N}.tmp");

        try
        {
            File.WriteAllLines(tempPath, names.Select(name => name.ToString()));
            File.Move(tempPath, destination, true);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
