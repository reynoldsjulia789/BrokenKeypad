using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

internal class Hash : IWordBank
{
    private readonly HashSet<string> hashSet = [];

    public bool Insert(string word)
    {
        return hashSet.Add(word);
    }

    public bool FindWord(string word)
    {
        return hashSet.Contains(word);
    }

    public List<string> GetAllWords()
    {
        return hashSet.ToList();
    }
}
