using System;
using System.Collections.Generic;

namespace IAccess.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Get the total occurrences of a pattern in search text.
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns></returns>
        int GetTotalOccurrences(string text);
    }
    public class SearchService : ISearchService
    {
        private readonly string _pattern;
        private readonly bool _ignoreCase;
        private Dictionary<char, int> lastOccurrenceOfPatternCharacters = new Dictionary<char, int> { };

        public SearchService(string pattern, bool ignoreCase = true)
        {
            _pattern = ignoreCase ? pattern.ToLower() : pattern;
            _ignoreCase = ignoreCase;

            Initialize();
        }

        /// <summary>
        /// Save the last occurrences of each character in the pattern
        /// </summary>
        private void Initialize()
        {
            char key;

            for (int i = 0; i < _pattern.Length; i++)
            {
                key = _pattern[i];
                if (lastOccurrenceOfPatternCharacters.ContainsKey(key))
                {
                    lastOccurrenceOfPatternCharacters[key] = i;
                }
                else
                {
                    lastOccurrenceOfPatternCharacters.Add(key, i);
                }
            }
        }

        /// <summary>
        /// Get the total occurrences of a pattern in search text.
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <returns></returns>
        public int GetTotalOccurrences(string text)
        {
            int totalOccurrences = 0;

            if (!string.IsNullOrEmpty(_pattern))
            {
                if(_ignoreCase)
                {
                    text = text.ToLower();
                }

                int lengthOfPattern = _pattern.Length;
                int lengthOfText = text.Length;
                int startIndex = 0;

                // Loop while there's still room for search term
                while (startIndex <= (lengthOfText - lengthOfPattern))
                {
                    // if we have a match at this position
                    int j = lengthOfPattern - 1;

                    /* Keep reducing index j of pattern while
                    characters of pattern and text are
                    matching at this shift startIndex */
                    while (j >= 0 && _pattern[j] == text[startIndex + j])
                    {
                        j--;
                    }

                    /* If the pattern is present at current
                    shift, then index j will become -1 after
                    the above loop */
                    if (j < 0)
                    {
                        totalOccurrences++;
                        // when pattern does not occur at the end of text
                        if (startIndex + lengthOfPattern < lengthOfText)
                        {
                            // Shift the pattern so that the next character in text aligns with the last occurrence of it in pattern.
                            startIndex += lastOccurrenceOfPatternCharacters.ContainsKey(text[startIndex + lengthOfPattern]) 
                                ? lengthOfPattern - lastOccurrenceOfPatternCharacters[text[startIndex + lengthOfPattern]] 
                                : 1;
                        }
                        else
                        {
                            startIndex++;
                        }
                    } else
                    {
                        /* Shift the pattern so that the unmatch character
                        in text aligns with the last occurrence of
                        it in pattern. The max function is used to
                        make sure that we get a positive shift.
                        We may get a negative shift if the last
                        occurrence of bad character in pattern
                        is on the right side of the current
                        character. */
                        startIndex += lastOccurrenceOfPatternCharacters.ContainsKey(text[startIndex + j]) 
                                ? Math.Max(1, j - lastOccurrenceOfPatternCharacters[text[startIndex + j]]) 
                                : (j + 1);
                    }
                }
            }
            return totalOccurrences;
        }
    }
}
