using System.Diagnostics.CodeAnalysis;

namespace NameSorter.Core;

/// <summary>
///     Parses a line by splitting on whitespace. The last word is the last name,
///     and the remaining words are the given names, in order.
/// </summary>
public static class NameParser
{
    /// <summary>Attempts to parse a whitespace-separated name.</summary>
    public static bool TryParse(
        string? line,
        [NotNullWhen(true)] out Name? name,
        [NotNullWhen(false)] out string? error)
    {
        name = null;

        if (string.IsNullOrWhiteSpace(line))
        {
            error = "A name cannot be null, empty, or whitespace-only.";
            return false;
        }

        string[] words = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int givenNameCount = words.Length - 1;

        if (givenNameCount is < Name.MinGivenNames or > Name.MaxGivenNames)
        {
            string wordLabel = words.Length == 1 ? "word" : "words";
            error = $"Expected {Name.MinGivenNames} to {Name.MaxGivenNames} given names and a last name, but found {words.Length} {wordLabel}.";
            return false;
        }

        name = new Name(words[..^1], words[^1]);
        error = null;
        return true;
    }
}
