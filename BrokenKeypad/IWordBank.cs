using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

internal interface IWordBank
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
    public bool FindWord(string word);

    /// <summary>
    /// Returns a list of all the words in the word bank
    /// </summary>
    /// <returns>all words in in the word bank</returns>
    public List<string> GetAllWords();
}
