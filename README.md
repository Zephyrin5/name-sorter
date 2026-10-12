# NameSorter

[![Build and test](https://github.com/Zephyrin5/name-sorter/actions/workflows/ci.yml/badge.svg)](https://github.com/Zephyrin5/name-sorter/actions/workflows/ci.yml)

A .NET 10 CLI that sorts names by last name, then given names, and writes the result to the console and `sorted-names-list.txt`.

## Run a downloaded build

Download the ZIP for your platform from the [latest release](https://github.com/Zephyrin5/name-sorter/releases/latest):

| Platform | Asset |
| --- | --- |
| Windows x64 | `name-sorter-win-x64.zip` |
| Linux x64 | `name-sorter-linux-x64.zip` |
| macOS Apple Silicon | `name-sorter-osx-arm64.zip` |

Extract it and open a terminal in the extracted folder. No .NET installation is required. A sample input is at `examples/unsorted-names-list.txt` in this repo; replace the path below with your own file and quote paths containing spaces.

**Windows (PowerShell):**

```powershell
.\name-sorter.exe .\unsorted-names-list.txt
```

**Linux / macOS:**

```bash
chmod +x ./name-sorter
./name-sorter ./unsorted-names-list.txt
```

## Build, test and run from source

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then run from the repository root:

```bash
dotnet restore NameSorter.slnx
dotnet build NameSorter.slnx --configuration Release --no-restore
dotnet test --solution NameSorter.slnx --configuration Release --no-build
dotnet run --project src/Cli --configuration Release --no-build -- examples/unsorted-names-list.txt
```

Replace the example path with your input file. GitHub Actions runs the build and tests on Windows and Linux, then publishes platform builds as workflow artifacts.

To publish locally, select a runtime identifier such as `linux-x64`, `win-x64` or `osx-arm64`:

```bash
dotnet publish src/Cli --configuration Release --runtime linux-x64 --self-contained -p:PublishSingleFile=true --output publish
```

## Behavior

- Each nonblank line must contain one to three given names followed by a last name.
- Blank lines are skipped. Leading and trailing whitespace is removed, and whitespace between name parts is normalized to single spaces. Spelling and casing are preserved.
- Names are sorted by last name, then each given name in order. A shorter matching sequence sorts first. Equal sort keys retain input order, and duplicates are preserved.
- Comparison uses `StringComparer.OrdinalIgnoreCase` for case-insensitive, non-linguistic ordering. For example, `Äbel` sorts after `Zeller`.
- Output overwrites `sorted-names-list.txt` in the **current working directory**, regardless of the input file's location. Empty or blank-only input produces an empty file.
- All input is validated before writing. Invalid input reports its physical line number to stderr, leaves existing output untouched, and prints no names.
- The output file is replaced only after a temporary file has been written successfully. Names are printed after the output file has been replaced.
- The final word is always the last name. Suffixes and multi-word surnames are not recognized: `John Smith Jr.` is read as given names `John Smith` and last name `Jr.`.

Exit codes: `0` success, `1` invalid arguments, `2` invalid input, `3` I/O failure.

## Structure and design

- **Core:** `Name` holds immutable, validated name parts; `NameParser` parses individual names; `NameComparer` defines ordering. No file or console I/O.
- **Cli:** `NameListParser` handles lines and line numbers; `NameSortingJob` coordinates parsing, sorting and output; `INameSink` abstracts file and console output. `Program.cs` configures the application and maps failures to exit codes.
- **Tests:** Unit tests cover the model, parsers, comparer, job and console sink. Integration tests cover the file sink and run the built CLI against real files, including a 1,000-name input.

Adding another output destination requires an `INameSink` implementation and an entry in the sink array in `Program.cs`.

## Note on the brief

The example input contains "Vaugh Lewis", but its expected output shows "Vaughn Lewis". The sample file and tests use "Vaughn" consistently; the sorter does not correct spelling.

