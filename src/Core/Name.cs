namespace NameSorter.Core;

/// <summary>
/// A person's name consisting of one to three given names followed by
/// a last name, with no whitespace within individual parts.
/// </summary>
public sealed class Name
{
    /// <summary>Fewest given names a name may have.</summary>
    public const int MinGivenNames = 1;

    /// <summary>Most given names a name may have.</summary>
    public const int MaxGivenNames = 3;
    
    /// <summary>Creates a validated name from its parts.</summary>
    /// <param name="givenNames">One to three given names.</param>
    /// <param name="lastName">The last name.</param>
    /// <exception cref="ArgumentNullException">
    /// Either argument is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The given-name count is invalid, or a name part is blank
    /// or contains whitespace.
    /// </exception>
    public Name(IEnumerable<string> givenNames, string lastName)
    {
        ArgumentNullException.ThrowIfNull(givenNames);
        ArgumentNullException.ThrowIfNull(lastName);
        
        string[] given = [.. givenNames];
        if (given.Length is < MinGivenNames or > MaxGivenNames)
            throw new ArgumentException($"A name needs {MinGivenNames} to {MaxGivenNames} given names.", nameof(givenNames));

        if (given.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Given names must not be blank.", nameof(givenNames));

        if (given.Any(part => part.Any(char.IsWhiteSpace)))
            throw new ArgumentException(
                "Each given name must be a single name part without whitespace.",
                nameof(givenNames));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));

        if (lastName.Any(char.IsWhiteSpace))
            throw new ArgumentException(
                "The last name must be a single name part without whitespace.",
                nameof(lastName));

        GivenNames = Array.AsReadOnly(given);
        LastName = lastName;
    }

    /// <summary>Given names in the order they were written. Contains 1 to 3 items.</summary>
    public IReadOnlyList<string> GivenNames { get; }

    /// <summary>The last name, used as the primary sort key.</summary>
    public string LastName { get; }

    /// <summary>The full name as given names then last name, separated by single spaces.</summary>
    public override string ToString()
    {
        return string.Join(" ", GivenNames.Append(LastName));
    }
}
