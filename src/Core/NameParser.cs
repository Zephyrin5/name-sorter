namespace NameSorter.Core;

/// <summary>
///     Parses a line by splitting on whitespace. The last token is the last name,
///     and the remaining tokens are the given names, in order.
/// </summary>
public static class NameParser
{
    /// <summary>Parses a whitespace-separated name.</summary>
    /// <exception cref="ArgumentNullException">
    /// The input is null.
    /// </exception>
    /// <exception cref="FormatException">
    /// The input does not contain a valid name.
    /// </exception>
    public static Name Parse(string nameString)
    {
        ArgumentNullException.ThrowIfNull(nameString);
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameString);
            string[] tokens = nameString.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            return new Name(tokens.SkipLast(1), tokens.Last());
        }
        catch (ArgumentException ex)
        {
            throw new FormatException($"'{nameString}' is not a valid name: {ex.Message}", ex);
        }
    }
}
