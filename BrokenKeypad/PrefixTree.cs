using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public class PrefixTree
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

}
