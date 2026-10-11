namespace NameSorter.Tests.Helpers;

internal static class NameGenerator
{
    public static string[] Generate(int count, int seed)
    {
        string[] givenPool = ["Ann", "Bob", "Cara", "Dev", "Eli", "Fay", "Gus", "Hana", "Ivan", "Jo"];
        string[] lastPool = ["Adams", "Baker", "Clarke", "Diaz", "Evans", "Frost", "Gray", "Hill", "Ito", "Jones"];
        Random random = new Random(seed);

        return
        [
            .. Enumerable.Range(0, count).Select(_ =>
            {
                int givenCount = random.Next(1, 4);
                var parts = Enumerable.Range(0, givenCount).Select(_ => givenPool[random.Next(givenPool.Length)])
                    .Append(lastPool[random.Next(lastPool.Length)]);
                string line = string.Join(" ", parts);
                return random.Next(4) == 0 ? line.ToUpperInvariant() : line;
            })
        ];
    }
}
