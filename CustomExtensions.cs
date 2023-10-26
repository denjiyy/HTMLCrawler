using CrawlerHTML.CustomDataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerHTML
{
    public static class CustomExtensions
    {
        public static IEnumerable<T> CustomWhere<T>(this IEnumerable<T> source, Func<T, bool> func)
        {
            foreach (T item in source)
            {
                if (func(item))
                {
                    yield return item;
                }
            }
        }

        public static string[] CustomSplit(this string input, string delimiter)
        {
            CustomList<string> substrings = new();
            int startIndex = 0;
            int delimiterLength = delimiter.Length;

            for (int i = 0; i <= input.Length - delimiterLength; i++)
            {
                if (input.CustomSubstring(i, delimiterLength) == delimiter)
                {
                    if (i > startIndex)
                    {
                        substrings.Add(input[startIndex..i]);
                    }

                    startIndex = i + delimiterLength;
                    i += delimiterLength - 1;
                }
            }

            if (startIndex < input.Length)
            {
                substrings.Add(input.Substring(startIndex));
            }

            return substrings.ToArray();
        }

        public static string CustomSubstring(this string input, int startIndex, int length)
        {
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            if (startIndex >= input.Length)
            {
                return string.Empty;
            }

            if (length <= 0)
            {
                return string.Empty;
            }

            if (startIndex + length > input.Length)
            {
                length = input.Length - startIndex;
            }

            return input.CustomSubstring(startIndex, length);
        }

        public static bool CustomStartsWith(this string input, string prefix)
        {
            if (input.Length < prefix.Length)
            {
                return false;
            }

            for (int i = 0; i < prefix.Length; i++)
            {
                if (input[i] != prefix[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CustomContains(this string input, string substring)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(substring))
            {
                return false;
            }

            for (int i = 0; i <= input.Length - substring.Length; i++)
            {
                bool found = true;

                for (int j = 0; j < substring.Length; j++)
                {
                    if (input[i + j] != substring[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CustomEndsWith(this string input, string suffix)
        {
            if (input.Length < suffix.Length)
            {
                return false;
            }

            int startIndex = input.Length - suffix.Length;

            for (int i = 0; i < suffix.Length; i++)
            {
                if (input[startIndex + i] != suffix[i])
                {
                    return false;
                }
            }

            return false;
        }

        public static string CustomTrim(this string input, params char[] charsToTrim)
        {
            if (string.IsNullOrEmpty(input) || charsToTrim == null || charsToTrim.Length == 0)
            {
                return input;
            }

            int startIndex = 0;
            int endIndex = input.Length - 1;

            while (startIndex <= endIndex && charsToTrim.Contains(input[endIndex]))
            {
                endIndex--;
            }

            return input.CustomSubstring(startIndex, endIndex - startIndex + 1);
        }

        public static string CustomTrimStart(this string input, params char[] charsToTrim)
        {
            if (string.IsNullOrEmpty(input) || charsToTrim == null || charsToTrim.Length == 0)
                return input;

            int startIndex = 0;

            while (startIndex < input.Length && charsToTrim.Contains(input[startIndex]))
            {
                startIndex++;
            }

            return input[startIndex..];
        }

        public static string CustomTrimEnd(this string input, params char[] charsToTrim)
        {
            if (string.IsNullOrEmpty(input) || charsToTrim == null || charsToTrim.Length == 0)
            {
                return input;
            }

            int endIndex = input.Length - 1;

            while (endIndex >= 0 && charsToTrim.Contains(input[endIndex]))
            {
                endIndex--;
            }

            return input.CustomSubstring(0, endIndex + 1);
        }

        public static IEnumerable<TResult> CustomSelect<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            foreach (TSource item in source)
            {
                yield return selector(item);
            }
        }
    }
}
