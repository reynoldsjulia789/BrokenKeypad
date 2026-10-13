using System.Diagnostics;
using System.Text.Json;

namespace BrokenKeypad;

public static class Program
{
    private enum Mode { ExhaustiveSearch, BranchAndBound }

    private enum DictionaryType { Hash, PrefixTree }

    private static class Settings
    {
        public static string DictionaryFilepath     = "../dictionary.txt";
        public static Mode SearchMode               = Mode.BranchAndBound;
        public static DictionaryType DictionaryType = DictionaryType.PrefixTree;
        public static bool PrintDictionary          = false;
        public static Dictionary<int, List<char>> IntCharMap = new()
        {
            [2] = [ 'a', 'b', 'c' ],
            [3] = [ 'd', 'e', 'f' ],
            [4] = [ 'g', 'h', 'i' ],
            [5] = [ 'j', 'k', 'l' ],
            [6] = [ 'm', 'n', 'o' ],
            [7] = [ 'p', 'q', 'r', 's' ],
            [8] = [ 't', 'u', 'v' ],
            [9] = [ 'w', 'x', 'y', 'z' ]
        };
    }
    
    private static class Runtimes
    {
        public static double? DictionaryCreationTime     = null;
        public static List<double?> SearchTimes          = [];
        public static double? GetAllWordsTime            = null;
        public static double? PrintAllWordsTime          = null;

    }


    // ---------- MAIN -----------


    public static void Main(string[] args)
    {
        // Setup
        PrintRunInstructions();
        ParseArgs(args);

        Stopwatch stopwatch = new();

        // Read Dictionary
        Console.WriteLine();
        Console.WriteLine($"Attempting to build dictionary from {Settings.DictionaryFilepath}");

        IWordBank    dictionary;
        List<string> allWords;

        try
        {
            stopwatch.Start();

            dictionary = BuildDictionary();

            stopwatch.Stop();
            Runtimes.DictionaryCreationTime = stopwatch.Elapsed.TotalNanoseconds;
        }
        catch (Exception caught)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"Program.Main: An error occurred while attempting to build dictionary");
            Console.WriteLine(caught.Message);

            Console.ResetColor();

            Console.WriteLine("Exiting program.");

            return;
        }

        // Get All Words From Dictionary
        stopwatch.Start();

        allWords = dictionary.GetAllWords();

        stopwatch.Stop();
        Runtimes.GetAllWordsTime = stopwatch.Elapsed.TotalNanoseconds;

        Console.WriteLine($"{Settings.DictionaryType} Dictionary built successfully with " +
            $"{allWords.Count:N0} word{((allWords.Count == 1) ? "" : "s")}");

        // Print Dictionary
        if (Settings.PrintDictionary is true)
        {
            stopwatch.Restart();

            PrintDictionary(allWords);

            stopwatch.Stop();
            Runtimes.PrintAllWordsTime = stopwatch.Elapsed.TotalNanoseconds;
        }

        // Search
        while (true)
        {
            List<int> input = GetUserInput();

            stopwatch.Restart();

            List<string> results = dictionary.GetPossibleWords(input, Settings.IntCharMap);

            stopwatch.Stop();
            Runtimes.SearchTimes.Add(stopwatch.Elapsed.TotalNanoseconds);

            PrintResults(results);

            if (UserHasMoreInputs() is false) break;
        }

        // Print Run Times
        PrintRuntimes();
    }


    // ---------- ARGS -----------


    /// <summary>
    /// Prints the instructions on how to run the program.
    /// Should be called before any args are parsed, since it prints the default settings.
    /// </summary>
    private static void PrintRunInstructions()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine();
        Console.WriteLine("To run:");
        Console.WriteLine("dotnet run [--filepath <path to dictionary file>] [--mode <search mode>] " +
            "[--dictionary-type <type>] [--print-dictionary]");

        Console.WriteLine($"* all args are optional");

        Console.WriteLine($"* filepath: path to .txt file containing the dictionary to use to verify word validity.");
        Console.WriteLine($"  default is {Settings.DictionaryFilepath}");

        Console.WriteLine($"* mode: the mode used to lookup words.");

        string[] options = Enum.GetNames(typeof(Mode))
            .Select(option => ToKebabCase(option))
            .ToArray();

        Console.WriteLine($"  options include: {string.Join(", ", options)}");
        Console.WriteLine($"  default is {ToKebabCase(Settings.SearchMode.ToString())}");

        Console.WriteLine($"* print-dictionary: if included, all the words in the dictionary being used will be " +
            $"printed to the console");

        Console.ResetColor();
    }

    /// <summary>
    /// Parses any existing args to set settings
    /// </summary>
    private static void ParseArgs(string[] args)
    {

        for (int idx = 0; idx < args.Length; idx++)
        {
            if (args[idx] == "--print-dictionary")
            {
                Settings.PrintDictionary = true;
            }

            if ((idx + 1) >= args.Length) continue;

            if (args[idx] == "--filepath")
            {
                Settings.DictionaryFilepath = args[idx + 1];
            }

            if (args[idx] == "--mode")
            {
                if (args[idx + 1] == "exhaustive-search")
                {
                    Settings.SearchMode     = Mode.ExhaustiveSearch;
                    Settings.DictionaryType = DictionaryType.Hash;
                }

                if (args[idx + 1] == "branch-and-bound")
                {
                    Settings.SearchMode     = Mode.BranchAndBound;
                    Settings.DictionaryType = DictionaryType.PrefixTree;
                }

                // add more mode options here
            }

            idx++; // skip idx + 1 since already seen
        }
    }


    // ---------- DICTIONARY -----------


    /// <summary>
    /// Builds a dictionary of words from the filepath specified in settings
    /// </summary>
    /// <returns>word bank with all the words in the dictionary file</returns>
    /// <exception cref="NotImplementedException">if dictionary type is not prefix tree</exception>
    private static IWordBank BuildDictionary()
    {
        IWordBank wordBank;

        if (Settings.DictionaryType == DictionaryType.PrefixTree)
        {
            wordBank = new PrefixTree();
        }
        else if (Settings.DictionaryType == DictionaryType.Hash)
        {
            wordBank = new Hash();
        }
        else
        {
            throw new NotImplementedException(
                $"Program.BuildDictionary: {Settings.DictionaryType} has not been implemented yet");
        }

        var file = File.ReadAllLines(Settings.DictionaryFilepath);

        foreach (var line in file)
        {
            _ = wordBank.Insert(line.Split(',')[0].ToLower());
        }

        return wordBank;
    }

    /// <summary>
    /// Prints all the words in the given dictionary
    /// </summary>
    private static void PrintDictionary(List<string> allWords)
    {
        Console.WriteLine();
        Console.WriteLine("Words:");
        Console.WriteLine();

        IEnumerable<string> wordsGroupedByLetter = allWords
            .GroupBy(word => word[0])
            .Select(group => string.Join(", ", group));

        foreach (string group in wordsGroupedByLetter)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{group[0]}: ");
            Console.ResetColor();

            Console.WriteLine(group);
            Console.WriteLine();
        }

        Console.WriteLine($"Dictionary Type:     {Settings.DictionaryType}");
        Console.WriteLine($"Dictionary Filepath: {Settings.DictionaryFilepath}");
        Console.WriteLine($"Word Count:          {allWords.Count:N0}");
    }


    // ---------- USER INPUT -----------


    /// <summary>
    /// Reads integers entered by the user and returns them as a list.
    /// </summary>
    /// <returns>A list of integers entered by the user. Returns an empty list if no valid integers were provided.</returns>
    private static List<int> GetUserInput()
    {
        List<int> input = [];

        int low  = Settings.IntCharMap.Keys.Min();
        int high = Settings.IntCharMap.Keys.Max();

        Console.WriteLine();
        Console.WriteLine("The following numbers correspond to the following letters.");

        foreach (var mapping in Settings.IntCharMap)
        {
            Console.WriteLine($"* {mapping.Key} = {string.Join(", ", mapping.Value)}");
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Please enter a sequence of numbers to represent a word:");
            Console.WriteLine("For example: the sequence 228 could represent act, bat, or cat");
            Console.Write(">  ");

            var rawInput   = Console.ReadLine()?.Trim();
            var errorMsg   = $"Invalid input. Please only enter didgits {low}-{high}";
            var validInput = true;

            if (string.IsNullOrWhiteSpace(rawInput))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errorMsg);
                Console.ResetColor();

                continue;
            }

            foreach (char character in rawInput)
            {
                if ((int.TryParse(character.ToString(), out int num) is false) || (num < low) || (num > high))
                {
                    input.Clear();

                    validInput = false;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(errorMsg);
                    Console.ResetColor();

                    break;
                }

                input.Add(num);
            }

            if (validInput is true) return input;
        }
    }

    /// <summary>
    /// Determines wheter the user has more sequences of numbers they would like to process
    /// </summary>
    /// <returns>true to continue, false to exit</returns>
    private static bool UserHasMoreInputs()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Would you like to input another sequence? (y/n)");
            Console.Write(">  ");

            var rawInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(rawInput)) continue;

            if (rawInput[0] == 'y' || rawInput[0] == 'Y') return true;

            return false;
        }
    }


    // ---------- OTHER -----------

    /// <summary>
    /// Prints the results of a search
    /// </summary>
    private static void PrintResults(List<string> results)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine();
        Console.WriteLine($"{((results.Count == 0) 
            ? "No results found" 
            : $"Possible words: {string.Join(", ", results)}")}");

        Console.ResetColor();
    }

    /// <summary>
    /// Prints available .NET runtimes and their version information to the console.
    /// </summary>
    /// <remarks>Writes runtime identifiers and version details in a human-readable form to standard output
    /// for diagnostic purposes.</remarks>
    private static void PrintRuntimes()
    {
        string defaultTime = "Not Recorded";

        Console.WriteLine();
        Console.WriteLine("Runtimes in nanoseconds:");

        Console.WriteLine($"Build Dictionary - {((Runtimes.DictionaryCreationTime is null) 
            ? defaultTime : $"{Runtimes.DictionaryCreationTime:N0}")}");

        Console.WriteLine($"Get All Words    - {((Runtimes.GetAllWordsTime is null) 
            ? defaultTime : $"{Runtimes.GetAllWordsTime:N0}")}");

        Console.WriteLine($"Print All Words  - {((Runtimes.PrintAllWordsTime is null)
            ? defaultTime : $"{Runtimes.PrintAllWordsTime:N0}")}");

        Console.WriteLine($"Search           - {((Runtimes.SearchTimes.Count == 0) 
            ? defaultTime : string.Join(", ", Runtimes.SearchTimes.Select(number => number?.ToString("N0"))))}");
    }

    /// <summary>
    /// Converts a string from PascalCase to kebab-case
    /// </summary>
    private static string ToKebabCase(string PascalCase)
    {
        if (string.IsNullOrEmpty(PascalCase)) return PascalCase;

        return JsonNamingPolicy.KebabCaseLower.ConvertName(PascalCase);
    }
}