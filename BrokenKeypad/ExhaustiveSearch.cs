using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

/// <summary>
/// Finds valid words for a keypad digit sequence by brute force: every possible letter combination
/// is generated, and each one is checked against the dictionary.
/// </summary>
/// <remarks>
/// <para>
/// No candidate is ever skipped early. Even when no word could possibly begin with the letters chosen
/// so far (e.g. "bq..."), the search still builds every full-length string from that prefix.
/// This makes runtime grow exponentially with input length, which is the trade-off
/// <see cref="BranchAndBound"/> is designed to improve on.
/// </para>
/// <para>
/// Intended to be paired with the <see cref="Hash"/> dictionary, since it only ever performs
/// whole-word lookups, each of which is O(1) in a hash set.
/// </para>
/// </remarks>
/// <param name="intCharMap">Maps each keypad digit to the letters it can represent (e.g. 2 -> a, b, c)</param>
/// <param name="dictionary">The word bank used to determine whether a candidate string is a valid word</param>
public class ExhaustiveSearch(Dictionary<int, List<char>> intCharMap, IWordBank dictionary) : ISearch
{
    private readonly IWordBank dictionary = dictionary;
    private readonly Dictionary<int, List<char>> intCharMap = intCharMap;

    /// <summary>
    /// Finds all valid words that can be spelled by the given sequence of keypad digits.
    /// </summary>
    /// <param name="numbers">The sequence of digits entered on the keypad. Every digit must be a key in the int-char map.</param>
    /// <returns>Every candidate string that the dictionary recognizes as a word, in keypad letter order</returns>
    /// <exception cref="KeyNotFoundException">Thrown if a digit has no letters mapped to it (e.g. 0 or 1)</exception>
    public List<string> FindWords(List<int> numbers)
    {
        // Start at the first digit with an empty candidate and no results.
        // A single StringBuilder is shared across the entire search, so no new strings are
        // allocated while building candidates, only when a finished one is checked.
        return FindWords(numbers, 0, new(), []);
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
    /// <param name="position">Index of the digit whose letter is being chosen in this call</param>
    /// <param name="builder">The candidate built so far. Its length always equals <paramref name="position"/>.</param>
    /// <param name="words">Accumulates the valid words found. Shared by all recursive calls.</param>
    /// <returns>The same <paramref name="words"/> list, so the top-level call can return it directly</returns>
    private List<string> FindWords(List<int> numbers, int position, StringBuilder builder, List<string> words)
    {
        // Base case: a letter has been chosen for every digit, so the builder holds a complete,
        // full-length candidate (a leaf of the tree). This is the only place the dictionary is checked.
        if (position == numbers.Count)
        {
            string candidate = builder.ToString();

            if (this.dictionary.FindWord(candidate)) words.Add(candidate);

            return words;
        }

        // Recursive case: try every letter the current digit can stand for
        foreach (char letter in this.intCharMap[numbers[position]])
        {
            // Choose: extend the candidate with this letter
            builder.Append(letter);

            // Explore: fill in the remaining digits. When this returns, every candidate that starts with the current prefix has been checked.
            FindWords(numbers, position + 1, builder, words);

            // Un-choose: remove the letter so the builder is back to what it was when this call started
            builder.Length--;
        }

        return words;
    }
}
