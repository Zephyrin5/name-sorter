namespace NameSorter.Tests.Helpers;

internal sealed class TempDirectory : IDisposable
{
    public string DirectoryPath { get; } =
        Directory.CreateDirectory(
            Path.Combine(Path.GetTempPath(), $"NameSorter tests {Guid.NewGuid():N}")).FullName;

    public string File(string name)
    {
        return Path.Combine(DirectoryPath, name);
    }

    public void Dispose()
    {
        Directory.Delete(DirectoryPath, true);
    }
}
