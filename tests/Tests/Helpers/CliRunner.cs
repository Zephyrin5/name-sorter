using System.Diagnostics;
using System.Text;

namespace NameSorter.Tests.Helpers;

internal static class CliRunner
{
    public static async Task<CliResult> RunAsync(string workingDirectory, params string[] args)
    {
        ProcessStartInfo info = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        info.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "name-sorter.dll"));
        foreach (string arg in args)
            info.ArgumentList.Add(arg);

        using Process process = Process.Start(info)
                                ?? throw new InvalidOperationException("Could not start the CLI.");

        // Read both streams concurrently so a full pipe can't block the process.
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();

        using CancellationTokenSource timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(true);
            await process.WaitForExitAsync(CancellationToken.None);
            throw;
        }

        return new CliResult(process.ExitCode, await stdout, await stderr);
    }
}
