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
    /// Finds valid words for a keypad digit sequence by brute force: every possible letter combination
    /// is generated, and each one is checked against the dictionary.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Paired with the hash set since it only ever performs whole-word lookups, each of which is O(1).
    /// </para>
    /// </remarks>
    /// <param name="numbers">The sequence of digits entered on the keypad. Every digit must be a key in the int-char map.</param>
    /// <param name="intCharMap">Maps each keypad digit to the letters it can represent (e.g. 2 -> a, b, c)</param>
    /// <returns>Every candidate string that the dictionary recognizes as a word, in keypad letter order</returns>
    /// <exception cref="KeyNotFoundException">Thrown if a digit has no letters mapped to it (e.g. 0 or 1)</exception>
    public List<string> GetPossibleWords(List<int> numbers, Dictionary<int, List<char>> intCharMap)
    {
        // Start at the first digit with an empty candidate and no results.
        // A single StringBuilder is shared across the entire search, so no new strings are
        // allocated while building candidates, only when a finished one is checked.
        return GetPossibleWords(numbers, intCharMap, 0, new(), []);
    }

    /// <summary>
    /// Recursively builds every letter combination for the digits from <paramref name="position"/> onward,
    /// appending each one that is a valid word to <paramref name="words"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tree is walked depth-first using backtracking: append a letter (choose), recurse to the next
    /// digit (explore), then remove the letter (un-choose) so the builder is back to how it was and
    /// the next sibling letter can be tried.
    /// </para>
    /// </remarks>
    /// <param name="numbers">The full sequence of digits being searched</param>
    /// <param name="intCharMap">Maps each keypad digit to the letters it can represent</param>
    /// <param name="position">Index of the digit whose letter is being chosen in this call</param>
    /// <param name="builder">The candidate built so far. Its length always equals <paramref name="position"/>.</param>
    /// <param name="words">Accumulates the valid words found. Shared by all recursive calls.</param>
    /// <returns>The same <paramref name="words"/> list, so the top-level call can return it directly</returns>
    private List<string> GetPossibleWords(List<int> numbers, Dictionary<int, List<char>> intCharMap,
        int position, StringBuilder builder, List<string> words)
    {
        // Base case: a letter has been chosen for every digit, so the builder holds a complete,
        // full-length candidate (a leaf of the tree). This is the only place the dictionary is checked.
        if (position == numbers.Count)
        {
            string candidate = builder.ToString();

            if (hashSet.Contains(candidate)) words.Add(candidate);

            return words;
        }

        // Recursive case: try every letter the current digit can stand for
        foreach (char letter in intCharMap[numbers[position]])
        {
            // Choose: extend the candidate with this letter
            builder.Append(letter);

            // Explore: fill in the remaining digits. When this returns, every candidate that starts with the current prefix has been checked.
            GetPossibleWords(numbers, intCharMap, position + 1, builder, words);

            // Un-choose: remove the letter so the builder is back to what it was when this call started
            builder.Length--;
        }

        return words;
    }

    public List<string> GetAllWords()
    {
        return hashSet.ToList();
    }
}
