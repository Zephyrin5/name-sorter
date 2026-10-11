# NameSorter

[![Build and test](https://github.com/zephyrin5/name-sorter/actions/workflows/ci.yml/badge.svg)](https://github.com/zephyrin5/name-sorter/actions/workflows/ci.yml)

A CLI that sorts names by last name, then given names, and writes the result to the screen and `sorted-names-list.txt`.

## Build, test and run

Install the .NET 10 SDK, then run from the repository root on Windows or Linux:

```bash
dotnet restore NameSorter.slnx
dotnet build NameSorter.slnx --configuration Release --no-restore
dotnet test --solution NameSorter.slnx --configuration Release --no-build
dotnet run --project src/Cli --configuration Release --no-build -- examples/unsorted-names-list.txt
```

Replace the example path with your input file; quote paths containing spaces. Tests use the xUnit v3 executable runner and return a nonzero exit code on failure. GitHub Actions runs the same build and test commands on Windows and Linux.

## Publish and run

To create a self-contained executable, publish for your platform.
This example targets Linux x64; use `win-x64` or `osx-arm64` as appropriate:

```bash
dotnet publish src/Cli --configuration Release --runtime linux-x64 --self-contained -p:PublishSingleFile=true --output publish

./publish/name-sorter examples/unsorted-names-list.txt
```

## Behaviour

- Each nonblank line must contain one to three given names followed by a last name.
- Blank lines are skipped. Leading and trailing whitespace is removed, and spaces or tabs between name parts are normalized to single spaces. Spelling and casing are preserved.
- Names are sorted by last name, then each given name in order. A shorter matching sequence sorts first. Equal sort keys retain input order, and duplicates are preserved.
- Comparison uses InvariantCultureIgnoreCase for case-insensitive, culture-invariant linguistic ordering; it does not follow language-specific alphabetical rules.
- Output replaces `sorted-names-list.txt` in the **current working directory**, regardless of the input file's location. Empty or blank-only input produces an empty file.
- All input is validated before writing. Invalid input reports its physical line number to stderr, leaves existing output untouched, and prints no names.
- Names are printed only after the output file has been written successfully.
- The last token on a line is always the last name. Suffixes such as "Jr." and multi-word surnames such as "Van Dyke" are not recognised: "John Smith Jr." is read as given names "John Smith" and last name "Jr.".

Exit codes: `0` success, `1` invalid arguments, `2` invalid input, `3` file read/write failure.

## Structure and design

- **Core:** `Name` holds immutable, validated name parts; `NameParser` parses individual names; `NameComparer` defines ordering. No file or console I/O.
- **Cli:** `NameListParser` handles lines and line numbers; `NameSortingJob` coordinates parsing, sorting and output; `INameSink` abstracts file and console output. `Program.cs` configures the application and maps failures to exit codes.
- **Tests:** Unit tests cover the model, parsers, comparer and console sink. Integration tests cover the file sink and run the built CLI against real files, including a 1,000-name input.

`INameSink` represents the two required output destinations. Adding another destination requires a new implementation and an entry in the sink array. Parsing and comparison are tested directly; file reading uses the standard library and is covered by integration tests.

## Notes on the brief

The example input contains "Vaugh Lewis", but its expected output shows "Vaughn Lewis". The sample file and tests use "Vaughn" consistently; the sorter does not correct spelling.
