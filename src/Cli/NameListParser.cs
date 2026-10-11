using NameSorter.Core;
namespace NameSorter.Cli;

/// <summary>
///  Parses a list of names, one per line. Use <see cref="NameParser" /> for a single
///  name.
/// </summary>
internal static class NameListParser
{
    /// <summary>
    /// Parses names from lines, skipping blank lines.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// The input collection is null.
    /// </exception>
    /// <exception cref="FormatException">
    /// A line is invalid. The message includes its line number.
    /// </exception>
    public static IReadOnlyList<Name> ParseLines(IEnumerable<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var names = new List<Name>();
        int lineNumber = 0;

        foreach (string line in lines)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                names.Add(NameParser.Parse(line));
            }
            catch (FormatException ex)
            {
                throw new FormatException(
                    $"Line {lineNumber}: {ex.Message}", ex);
            }
        }

        return names.AsReadOnly();
    }
}
