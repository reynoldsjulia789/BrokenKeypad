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
            words  .Add(builder.ToString());
            builder.Clear();
        }

        foreach (var child in curr.Children)
        {
            builder.Append(child.Key);

            GetAllWords(child.Value, builder, words);
        }

        return words;
    }

    public bool FindWord(string word)
    {
        return FindWord(this.Root, word, 0);
    }

    private bool FindWord(Node curr, string word, int charIdx)
    {
        if (charIdx >= word.Length) return false;

        _ = curr.Children.TryGetValue(word[charIdx], out Node? child);

        if ((child is not null) && (charIdx == (word.Length - 1))) return true;

        if (child is null) return false;

        return FindWord(child, word, charIdx + 1);
    }
}
