using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public class PrefixTree : IWordBank
{
    private class Node()
    {
        public SortedDictionary<char, Node> Children { get; init; } = new();
        public bool IsWord { get; set; } = false;
    }

    private readonly Node Root = new();

    public bool Insert(string word)
    {
        Node  curr = this.Root;

        foreach (var letter in word)
        {
            if (curr.Children.TryGetValue(letter, out Node? next) is false)
            {
                next = new();
                curr.Children[letter] = next;
            }

            curr = next;
        }

        curr.IsWord = true;

        return true;
    }

    public List<string> GetAllWords()
    {
        return GetAllWords(this.Root, new(), []);
    }

    private List<string> GetAllWords(Node curr, StringBuilder builder, List<string> words)
    {
        if (curr.IsWord) 
        {
            words.Add(builder.ToString());
        }

        foreach (var child in curr.Children)
        {
            builder.Append(child.Key);

            GetAllWords(child.Value, builder, words);

            builder.Length--;
        }

        return words;
    }

    public bool WordExists(string word)
    {
        return WordExists(this.Root, word, 0);
    }

    private bool WordExists(Node curr, string word, int charIdx)
    {
        if (charIdx >= word.Length) return false;

        _ = curr.Children.TryGetValue(word[charIdx], out Node? child);

        if ((child is not null) && (charIdx == (word.Length - 1))) return true;

        if (child is null) return false;

        return WordExists(child, word, charIdx + 1);
    }

    /// <summary>
    /// Uses Branch and Bound to search for possible words, and returns the results as a list
    /// </summary>
    public List<string> GetPossibleWords(List<int> numbers, Dictionary<int, List<char>> intCharMap)
    {
        return BranchAndBound(this.Root, numbers, 0, new(), intCharMap, []);
    }

    private List<string> BranchAndBound(
        Node curr,
        List<int> numbers, 
        int currIdx,
        StringBuilder builder,
        Dictionary<int, List<char>> intCharMap, 
        List<string> results)
    {
        if (currIdx == numbers.Count)
        {
            if (curr.IsWord) results.Add(builder.ToString());
            return results;
        }

        List<char> possibleChars = intCharMap[numbers[currIdx]];

        foreach (var possibleChar in possibleChars)
        {
            if (curr.Children.TryGetValue(possibleChar, out Node? next) is false) continue; 

            builder.Append(possibleChar);

            BranchAndBound(next, numbers, currIdx + 1, builder, intCharMap, results);

            builder.Length--;
        }

        return results;
    }
}
