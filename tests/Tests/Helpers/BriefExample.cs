namespace NameSorter.Tests.Helpers;

internal static class BriefExample
{
    public static readonly string[] Input =
    [
        "Janet Parsons", "Vaughn Lewis", "Adonis Julius Archer", "Shelby Nathan Yoder",
        "Marin Alvarez", "London Lindsey", "Beau Tristan Bentley", "Leo Gardner",
        "Hunter Uriah Mathew Clarke", "Mikayla Lopez", "Frankie Conner Ritter"
    ];

    public static readonly string[] Expected =
    [
        "Marin Alvarez", "Adonis Julius Archer", "Beau Tristan Bentley",
        "Hunter Uriah Mathew Clarke", "Leo Gardner", "Vaughn Lewis", "London Lindsey",
        "Mikayla Lopez", "Janet Parsons", "Frankie Conner Ritter", "Shelby Nathan Yoder"
    ];
}
