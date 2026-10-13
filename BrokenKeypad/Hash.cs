using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public class Hash : IWordBank
{
    private readonly HashSet<string> hashSet = [];

    public bool Insert(string word)
    {
        return hashSet.Add(word);
    }

    public bool WordExists(string word)
    {
        return hashSet.Contains(word);
    }

    /// <summary>
    /// Uses exhaustive search to create a list of the possible words given a list of numbers and their
    /// coresponding possible chars
    /// </summary>
    public List<string> GetPossibleWords(List<int> numbers, Dictionary<int, List<char>> intCharMap)
    {
        return ["Hash.GetPossibleWords: Exhaustive search not yet implemented"];
    }

    public List<string> GetAllWords()
    {
        return hashSet.ToList();
    }
}
