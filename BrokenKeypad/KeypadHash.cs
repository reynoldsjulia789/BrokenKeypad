using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public class KeypadHash : IWordBank
{

    private readonly Dictionary<string, List<string>> hashMap = [];   // "228" -> [act, bat, cat]
    private readonly Dictionary<char, int> charIntMap = [];           // 'c' -> 2

    public KeypadHash(Dictionary<int, List<char>> intCharMap)
    {
        foreach (var mapping in intCharMap)
            foreach (char letter in mapping.Value)
                charIntMap[letter] = mapping.Key;
    }

}