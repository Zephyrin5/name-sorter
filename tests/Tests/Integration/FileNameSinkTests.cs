using NameSorter.Cli;
using NameSorter.Core;
using NameSorter.Tests.Helpers;
using System.Collections;

namespace NameSorter.Tests.Integration;

public class FileNameSinkTests
{
    private const string OutputFile = "out.txt";

    [Fact]
    public void Write_WritesOneNamePerLineInOrder()
    {
        using TempDirectory dir = new TempDirectory();
        Name[] names = [new Name(["Ann"], "Lee"), new Name(["Bob", "Jay"], "Smith")];

        new FileNameSink(dir.File(OutputFile)).Write(names);

        Assert.Equal(["Ann Lee", "Bob Jay Smith"], File.ReadAllLines(dir.File(OutputFile)));
    }

    [Fact]
    public void Write_ExistingFile_IsOverwrittenNotAppended()
    {
        using TempDirectory dir = new TempDirectory();
        File.WriteAllText(dir.File(OutputFile), "old content that is longer than the new content\nsecond old line\n");

        new FileNameSink(dir.File(OutputFile)).Write([new Name(["Ann"], "Lee")]);

        Assert.Equal(["Ann Lee"], File.ReadAllLines(dir.File(OutputFile)));
    }

    [Fact]
    public void Write_NoNames_OverwritesExistingFileWithEmptyContent()
    {
        using TempDirectory dir = new TempDirectory();
        File.WriteAllText(dir.File(OutputFile), "old output");

        new FileNameSink(dir.File(OutputFile)).Write([]);

        Assert.Equal(0L, new FileInfo(dir.File(OutputFile)).Length);
    }

    [Fact]
    public void Write_AccentedNames_PreservesCharacters()
    {
        using TempDirectory dir = new TempDirectory();

        new FileNameSink(dir.File(OutputFile)).Write([new Name(["José"], "Núñez")]);

        Assert.Equal(["José Núñez"], File.ReadAllLines(dir.File(OutputFile)));
    }

    [Fact]
    public void Write_PathIsDirectory_ThrowsFileError()
    {
        using TempDirectory dir = new TempDirectory();
        string targetPath = dir.File(OutputFile);
        Directory.CreateDirectory(targetPath);

        Exception? exception = Record.Exception(() => new FileNameSink(targetPath).Write([new Name(["Ann"], "Lee")]));

        Assert.True(
            exception is IOException or UnauthorizedAccessException,
            $"Unexpected exception: {exception}");
    }
    
    [Fact]
    public void Write_FailsMidWrite_KeepsExistingFileAndLeavesNoTempFiles()
    {
        using TempDirectory dir = new TempDirectory();
        File.WriteAllText(dir.File(OutputFile), "previous output");

        Assert.Throws<IOException>(
            () => new FileNameSink(dir.File(OutputFile)).Write(new FailsAfterFirstName()));

        Assert.Equal("previous output", File.ReadAllText(dir.File(OutputFile)));
        Assert.Equal([dir.File(OutputFile)], Directory.GetFiles(dir.DirectoryPath));
    }

    /// <summary>Yields one name, then fails, as a disk error mid-write would.</summary>
    private sealed class FailsAfterFirstName : IReadOnlyList<Name>
    {
        public int Count => 2;

        public Name this[int index] =>
            index == 0 ? new Name(["Ann"], "Lee") : throw new IOException("Simulated failure.");

        public IEnumerator<Name> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
