using NameSorter.Tests.Helpers;
using System.Diagnostics;
using System.Text;

namespace NameSorter.Tests.Integration;

public class ProgramTests
{
    private const string OutputFile = "sorted-names-list.txt";
    
    // Successful sorting and output
    [Fact]
    public async Task Run_BriefExample_PrintsNamesAndWritesFileInWorkingDirectory()
    {
        using TempDirectory dir = new TempDirectory();
        string inputDirectory = Directory.CreateDirectory(dir.File("input folder")).FullName;
        string input = Path.Combine(inputDirectory, "names list.txt");
        File.WriteAllLines(input, BriefExample.Input);

        CliResult result = await RunAsync(dir.DirectoryPath, input);

        AssertSuccess(dir, result, BriefExample.Expected);
        Assert.False(File.Exists(Path.Combine(inputDirectory, OutputFile)));
    }

    [Fact]
    public async Task Run_ExistingOutput_IsOverwritten()
    {
        using TempDirectory dir = new TempDirectory();
        File.WriteAllLines(dir.File("in.txt"), ["Bob Lee", "Ann Lee"]);
        await File.WriteAllTextAsync(dir.File(OutputFile), "stale content longer than the replacement\nextra line\n");

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        AssertSuccess(dir, result, ["Ann Lee", "Bob Lee"]);
    }

    [Fact]
    public async Task Run_ThousandNames_SortsAndPreservesEveryName()
    {
        using TempDirectory dir = new TempDirectory();
        string[] input = NameGenerator.Generate(1000, 42);
        File.WriteAllLines(dir.File("in.txt"), input);
        // Independent expected ordering; does not call the production parser or comparer.
        string[] expected =
        [
            .. input
                .Select(line => new { Line = line, Parts = line.Split(' ') })
                .OrderBy(x => x.Parts[^1], StringComparer.InvariantCultureIgnoreCase)
                .ThenBy(x => x.Parts[0], StringComparer.InvariantCultureIgnoreCase)
                .ThenBy(x => x.Parts.Length > 2 ? x.Parts[1] : "", StringComparer.InvariantCultureIgnoreCase)
                .ThenBy(x => x.Parts.Length > 3 ? x.Parts[2] : "", StringComparer.InvariantCultureIgnoreCase)
                .Select(x => x.Line)
        ];

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        AssertSuccess(dir, result, expected);
    }

    // Input variations
    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public async Task Run_BlankLinesAndWhitespace_AreHandled(string newline)
    {
        using TempDirectory dir = new TempDirectory();
        await File.WriteAllTextAsync(dir.File("in.txt"),
            string.Join(newline, "Bob Lee", "", " \t ", "  Ann\tMarie   Lee  ", ""));

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        AssertSuccess(dir, result, ["Ann Marie Lee", "Bob Lee"]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("\n \t\n")]
    public async Task Run_NoNames_OverwritesExistingOutputWithEmptyFile(string input)
    {
        using TempDirectory dir = new TempDirectory();
        await File.WriteAllTextAsync(dir.File("in.txt"), input);
        await File.WriteAllTextAsync(dir.File(OutputFile), "old output");

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        AssertSuccess(dir, result, []);
        Assert.Equal(0L, new FileInfo(dir.File(OutputFile)).Length);
    }

    [Fact]
    public async Task Run_EqualSortKeys_PreservesInputOrderAndDuplicates()
    {
        using TempDirectory dir = new TempDirectory();
        string[] input = ["ann lee", "Ann Lee", "ANN LEE", "Ann Lee"];
        File.WriteAllLines(dir.File("in.txt"), input);

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        AssertSuccess(dir, result, input);
    }

    [Fact]
    public async Task Run_AccentedNames_PreservesCharacters()
    {
        using TempDirectory dir = new TempDirectory();
        File.WriteAllLines(dir.File("in.txt"), ["José Núñez", "Zoë Müller"]);

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        AssertSuccess(dir, result, ["Zoë Müller", "José Núñez"]);
    }

    // Invalid arguments and input

    public static TheoryData<string, string[]> InvalidArguments =>
        new TheoryData<string, string[]>
        {
            { "No arguments", [] },
            { "Empty path", [""] },
            { "Whitespace-only path", ["   "] },
            { "Multiple paths", ["one.txt", "two.txt"] }
        };
    
    [Theory]
    [MemberData(nameof(InvalidArguments))]
    public async Task Run_InvalidArguments_ReturnsUsageError(string description, string[] args)
    {
        using TempDirectory dir = new TempDirectory();

        CliResult result = await RunAsync(dir.DirectoryPath, args);

        Assert.True(result.ExitCode == 1, $"{description}: exit code {result.ExitCode}: {result.Stderr}");
        Assert.Contains("Usage:", result.Stderr);
        Assert.Empty(result.Stdout);
        Assert.False(File.Exists(dir.File(OutputFile)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Run_InvalidLine_ReportsPhysicalLineNumberAndLeavesOutputUntouched(bool existingOutput)
    {
        using TempDirectory dir = new TempDirectory();
        File.WriteAllLines(dir.File("in.txt"), ["Ann Lee", "", "   ", "Madonna"]);
        if (existingOutput)
            await File.WriteAllTextAsync(dir.File(OutputFile), "previous run");

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        Assert.Equal(2, result.ExitCode);
        Assert.Contains("Line 4:", result.Stderr);
        Assert.Empty(result.Stdout);
        if (existingOutput)
            Assert.Equal("previous run", await File.ReadAllTextAsync(dir.File(OutputFile)));
        else
            Assert.False(File.Exists(dir.File(OutputFile)));
    }

    // I/O failures

    [Fact]
    public async Task Run_MissingInput_ReturnsFileErrorWithoutOutput()
    {
        using TempDirectory dir = new TempDirectory();

        CliResult result = await RunAsync(dir.DirectoryPath, "missing.txt");

        Assert.Equal(3, result.ExitCode);
        Assert.Contains("An I/O error occurred.", result.Stderr);
        Assert.Empty(result.Stdout);
        Assert.False(File.Exists(dir.File(OutputFile)));
    }

    [Fact]
    public async Task Run_OutputPathIsDirectory_ReturnsFileErrorBeforePrintingNames()
    {
        using TempDirectory dir = new TempDirectory();
        await File.WriteAllTextAsync(dir.File("in.txt"), "Ann Lee");
        Directory.CreateDirectory(dir.File(OutputFile));

        CliResult result = await RunAsync(dir.DirectoryPath, "in.txt");

        Assert.Equal(3, result.ExitCode);
        Assert.Contains("An I/O error occurred.", result.Stderr);
        Assert.Empty(result.Stdout);
        Assert.True(Directory.Exists(dir.File(OutputFile)));
    }
    
    // Helpers
    private sealed record CliResult(int ExitCode, string Stdout, string Stderr);

    private static async Task<CliResult> RunAsync(string workingDirectory, params string[] args)
    {
        string dll = Path.Combine(AppContext.BaseDirectory, "name-sorter.dll");
        foreach (string file in new[]
                 {
                     "name-sorter.dll",
                     "name-sorter.deps.json",
                     "name-sorter.runtimeconfig.json",
                     "NameSorter.Core.dll"
                 })
            Assert.True(File.Exists(Path.Combine(AppContext.BaseDirectory, file)),
                $"CLI launch file is missing: {file}");

        ProcessStartInfo info = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        info.ArgumentList.Add(dll);
        foreach (string arg in args)
            info.ArgumentList.Add(arg);

        using Process process = Process.Start(info)
                                ?? throw new InvalidOperationException("Could not start the CLI.");
        using CancellationTokenSource timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
        var stderr = process.StandardError.ReadToEndAsync(timeout.Token);

        try
        {
            await process.WaitForExitAsync(timeout.Token);
            return new CliResult(process.ExitCode, await stdout, await stderr);
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            if (!process.HasExited)
                process.Kill(true);
            await process.WaitForExitAsync(CancellationToken.None);
            try
            {
                await Task.WhenAll(stdout, stderr);
            }
            catch (OperationCanceledException)
            {
            }
            throw new TimeoutException("The CLI did not finish within 30 seconds.");
        }
    }

    private static void AssertSuccess(TempDirectory dir, CliResult result, string[] expected)
    {
        Assert.True(result.ExitCode == 0, $"Exit code {result.ExitCode}: {result.Stderr}");
        Assert.Empty(result.Stderr);
        Assert.Equal(expected, File.ReadAllLines(dir.File(OutputFile)));
        string expectedConsole = string.Concat(expected.Select(line => line + Environment.NewLine));
        Assert.Equal(expectedConsole, result.Stdout);
    }
}
