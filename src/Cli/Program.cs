using NameSorter.Cli;
using System.Text;
const string outputFileName = "sorted-names-list.txt";

if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
{
    Console.Error.WriteLine("Usage: name-sorter <path-to-names-file>");
    return 1;
}

try
{
    // Must be set before ConsoleNameSink is created, otherwise it'll use the default platform encoding
    Console.OutputEncoding = new UTF8Encoding(false);
    // Order matters: the file sink runs first, so names are printed only after the file was written.
    INameSink[] sinks = [new FileNameSink(outputFileName), new ConsoleNameSink()];

    new NameSortingJob(sinks).Run(File.ReadLines(args[0]));

    return 0;
}
catch (FormatException ex)
{
    Console.Error.WriteLine($"Invalid input. {ex.Message}");
    return 2;
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"An I/O error occurred. {ex.Message}");
    return 3;
}
