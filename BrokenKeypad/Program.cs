using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BrokenKeypad;

public static class Program
{
    private enum Mode
    {
        ExhaustiveSearch,
        BranchAndBound
    }

    private enum DictionaryType
    {
        HashTable,
        PrefixTree
    }

    private static class Settings
    {
        public static string DictionaryFilepath     = "../dictionary.txt";
        public static Mode SearchMode               = Mode.BranchAndBound;
        public static DictionaryType DictionaryType = DictionaryType.PrefixTree;
        public static bool PrintDictionary          = false;
    }
    
    private static class Runtimes
    {
        public static long DictionaryCreationTime { get; set; } = -1;
        public static long SearchTime             { get; set; } = -1;
        public static long PrintAllWordsTime      { get; set; } = -1;
    }

    public static void Main(string[] args)
    {
        // Setup
        PrintRunInstructions();
        ParseArgs(args);

        Stopwatch stopwatch = new();

        // Read Dictionary
        Console.WriteLine();
        Console.WriteLine($"Attempting to build dictionary from {Settings.DictionaryFilepath}");

        IWordBank dictionary;

        try
        {
            stopwatch.Start();

            dictionary = BuildDictionary();

            stopwatch.Stop();
            Runtimes.DictionaryCreationTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"{Settings.DictionaryType} Dictionary built successfully");
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

        // Print Dictionary
        if (Settings.PrintDictionary is true)
        {
            stopwatch.Restart();

            PrintDictionary(dictionary);

            stopwatch.Stop();
            Runtimes.PrintAllWordsTime = stopwatch.ElapsedMilliseconds;
        }


        // Search

        // Print Run Times
        PrintRuntimes();
    }

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
        Console.WriteLine($"  options include: exhaustive-search, branch-and-bound");
        Console.WriteLine($"  default is {Settings.SearchMode}");
        Console.WriteLine($"* dictionary-type: the data structure used to store the dictionary.");
        Console.WriteLine($"  options include: prefix-tree, hash-table");
        Console.WriteLine($"  default is {Settings.DictionaryType}");
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
                    Settings.SearchMode = Mode.ExhaustiveSearch;
                }

                if (args[idx + 1] == "branch-and-bound")
                {
                    Settings.SearchMode = Mode.BranchAndBound;
                }

                // add more mode options here
            }

            if (args[idx] == "--dictionary-type")
            {
                if (args[idx + 1] == "prefix-tree")
                {
                    Settings.DictionaryType = DictionaryType.PrefixTree;
                }

                if (args[idx + 1] == "hash-table")
                {
                    Settings.DictionaryType = DictionaryType.HashTable;
                }
            }

            idx++; // skip idx + 1 since already seen
        }
    }
    
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
        else
        {
            throw new NotImplementedException(
                $"Program.BuildDictionary: {Settings.DictionaryType} has not been implemented yet");
        }

        var file = File.ReadAllLines(Settings.DictionaryFilepath);

        foreach (var line in file)
        {
            _ = wordBank.Insert(line.Split(',')[0]);
        }

        return wordBank;
    }

    /// <summary>
    /// Prints all the words in the given dictionary
    /// </summary>
    private static void PrintDictionary(IWordBank dictionary)
    {
        var allWords = dictionary.GetAllWords();

        Console.WriteLine();
        Console.WriteLine("Words:");
        Console.WriteLine();
        Console.WriteLine(string.Join(',', allWords));
        Console.WriteLine();
        Console.WriteLine($"Dictionary Type:     {Settings.DictionaryType}");
        Console.WriteLine($"Dictionary Filepath: {Settings.DictionaryFilepath}");
        Console.WriteLine($"Word Count:          {allWords.Count:N0}");
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
        Console.WriteLine("Runtimes in ms:");

        Console.WriteLine($"Build Dictionary - {((Runtimes.DictionaryCreationTime == -1) 
            ? defaultTime : Runtimes.DictionaryCreationTime)}");

        Console.WriteLine($"Print All Words  - {((Runtimes.PrintAllWordsTime == -1) 
            ? defaultTime : Runtimes.PrintAllWordsTime)}");

        Console.WriteLine($"Search           - {((Runtimes.SearchTime == -1) 
            ? defaultTime : Runtimes.SearchTime)}");
    }
}