using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenKeypad;

public interface ISearch
{
    /// <summary>
    /// Finds all valid words form a list of ints.
    /// </summary>
    /// <param name="numbers">The sequence of numbers with which to determine possible valid words</param>
    /// <returns>List of valid words according to the provided dictionary</returns>
    public List<string> FindWords(List<int> numbers);
}
