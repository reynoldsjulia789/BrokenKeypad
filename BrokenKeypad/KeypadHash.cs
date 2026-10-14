using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public class KeypadHash : IWordBank
{

    private readonly Dictionary<string, List<string>> hashMap = [];   // "228" -> [act, bat, cat]
    private readonly Dictionary<char, int> charIntMap = [];           // 'c' -> 2

    /// <summary>
    /// Constructs a new KeypadHash with the given mapping of keypad digits to letters. The mapping is used to build a reverse map of letters to digits, which is used to convert words into their corresponding digit sequences.
    /// </summary>
    /// <param name="intCharMap"></param>
    public KeypadHash(Dictionary<int, List<char>> intCharMap)
    {
        foreach (var mapping in intCharMap)
            foreach (char letter in mapping.Value)
                charIntMap[letter] = mapping.Key;
    }

    /// <summary>
    /// Returns a list of all words stored in the hash map.
    /// The order of the words in the list is not guaranteed.
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllWords()
    {
        // return all words in the hash map by flattening the values of the dictionary into a single list
        //costs O(n) time and space, where n is the total number of words in the hash map
        var allWords = new List<string>();
        foreach (var words in hashMap.Values)
            allWords.AddRange(words);
        return allWords;
    }

    /// <summary>
    /// Finds valid words for a keypad digit sequence by looking up the sequence in the hash map. 
    /// If the sequence is found, the corresponding list of words is returned. 
    /// If the sequence is not found, an empty list is returned.
    /// </summary>
    /// <param name="numbers"></param>
    /// <param name="intCharMap"></param>
    /// <returns></returns>
    public List<string> GetPossibleWords(List<int> numbers, Dictionary<int, List<char>> intCharMap)
    {
        // Convert the list of numbers into a string representation of the digit sequence
        var digitSequence = new StringBuilder();
        foreach (int number in numbers)
        {
            digitSequence.Append(number);
        }

        // query the hash map for that sequence
        // return the list of words if found, or an empty list if not found.
        if (hashMap.TryGetValue(digitSequence.ToString(), out List<string>? words))
        {
            return words;
        }

        return new List<string>();
    }

    /// <summary>
    /// Inserts a word into the hash map, mapping its corresponding digit sequence to the word. 
    /// If the digit sequence already exists in the hash map, the word is added to the existing list of words 
    /// for that sequence. If the digit sequence does not exist, a new list is created and the word is added to it.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public bool Insert(string word)
    {
        //map letters to digits via charIntMap
        var digitSequence = new StringBuilder();
        foreach (char letter in word)
        {
            if (charIntMap.TryGetValue(letter, out int digit))
                digitSequence.Append(digit);
        }

        //TryGetValue for key/value pair
        if (!hashMap.TryGetValue(digitSequence.ToString(), out List<string>? words))
        {
            //if not key, make new list and add word to it
            words = new List<string>();
            hashMap[digitSequence.ToString()] = words;
        }

        //add word to list
        words.Add(word);
        return true;
    }

    /// <summary>
    /// Checks if a word exists in the hash map by converting it to its corresponding digit sequence 
    /// and checking if that sequence is a key in the hash map. If the sequence is found, 
    /// it checks if the word is in the list of words for that sequence.
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public bool WordExists(string word)
    {
        //map letters to digits via charIntMap
        var digitSequence = new StringBuilder();
        foreach (char letter in word)
        {
            if (!charIntMap.TryGetValue(letter, out int digit))
                return false;
            digitSequence.Append(digit);
        }

        //check if digit sequence is in hashMap
        if (hashMap.TryGetValue(digitSequence.ToString(), out List<string>? words))
        {
            //if digit sequence is in hashMap, check if word is in list of words for that sequence
            return words.Contains(word);
        }

        //if word is not in list, return false
        return false;
    }
}