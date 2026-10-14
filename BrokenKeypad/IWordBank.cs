using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public interface IWordBank
{
    /// <summary>
    /// Inserts a word into the word bank
    /// </summary>
    /// <param name="word">the word to insert</param>
    /// <returns>true if word was inserted, false if not</returns>
    public bool Insert(string word);

    /// <summary>
    /// Searches for a word in the word bank
    /// </summary>
    /// <param name="word">the word to find</param>
    /// <returns>true if word was found, false if not</returns>
    public bool WordExists(string word);

    /// <summary>
    /// Searches for possible words given a sequence of numbers and a mapping
    /// </summary>
    /// <param name="numbers">the list of numbers for which to find all possible words</param>
    /// <param name="intCharMap">mapping for which chars are represented by which ints</param>
    /// <returns>a list of all the possible valid words</returns>
    public List<string> GetPossibleWords(List<int> numbers, Dictionary<int, List<char>> intCharMap);

    /// <summary>
    /// Returns a list of all the words in the word bank
    /// </summary>
    /// <returns>all words in in the word bank</returns>
    public List<string> GetAllWords();
}
