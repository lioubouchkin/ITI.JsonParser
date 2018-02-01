using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITI.JsonParser.Correction
{

    public static class Parser
    {
        static CultureInfo _culture = CultureInfo.CreateSpecificCulture("en-US");
        static Regex _regex = new Regex("(?<!\\\\)\"");

        static string findStringOfValue(string value, ref int start, ref int count)
        {
            string _found = new string(value.Substring(start).TakeWhile<char>((c, index) => !c.Equals(',') && !c.Equals(']') && !c.Equals('}')).ToArray());
            MoveStart(ref start, _found, ref count);

            return _found;
        }

        private static void MoveStart(ref int start, string found, ref int count)
        {
            start = (start + found.Length <= count) ? start + found.Length : throw new IndexOutOfRangeException();
        }

        static string findStringOfString(string value, ref int start, ref int count)
        {
            string _found = _regex.Match(value.Substring(start)).ToString();
            MoveStart(ref start, _found, ref count);

            return _found;
        }

        public static bool ParseBoolean(string value, ref int start, ref int count)
        {
            Boolean.TryParse(findStringOfValue(value, ref start, ref count), out bool _result);

            return _result;
        }

        public static double ParseDouble(string value, ref int start, ref int count)
        {
            Double.TryParse(findStringOfValue(value, ref start, ref count), NumberStyles.Number, _culture, out double _result);

            return _result;
        }

        public static string ParseString(string value, ref int start, ref int count)
        {
            return findStringOfString(value, ref start, ref count);
        }

        public static object[] ParseArray(string value, ref int start, ref int count)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<String, Object> ParseObject(string value, ref int start, ref int count)
        {
            throw new NotImplementedException();
        }
    }
}
