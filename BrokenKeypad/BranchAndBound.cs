using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public class BranchAndBound(Dictionary<int, List<char>> intCharMap, IWordBank dictionary) : ISearch
{
    private readonly IWordBank dictionary = dictionary;
    private readonly Dictionary<int, List<char>> intCharMap = intCharMap;

    public List<string> FindWords(List<int> numbers)
    {
        return ["BranchAndBound.FindWords: Not implemented yet"];
    }
}
