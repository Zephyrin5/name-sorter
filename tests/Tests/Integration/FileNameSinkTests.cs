using NameSorter.Cli;
using NameSorter.Core;
using NameSorter.Tests.Helpers;

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
}
