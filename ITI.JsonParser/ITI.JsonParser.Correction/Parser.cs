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
            StringBuilder _builder = new StringBuilder();
            char _next;

            do
            {
                _builder.Append(value[start]);
                _next = start < count ? value[start + 1] : '\0';
            } while (Move(1, ref start, count) && !_next.Equals(',') && !_next.Equals(']') && !_next.Equals('}'));

            count -= _builder.Length;

            return _builder.ToString();
        }

        private static bool Move(int length, ref int start, int count)
        {
            if (start + length > count)
            {
                return false;
            }

            start += length;

            return true;
        }

        static string findStringOfString(string value, ref int start, ref int count)
        {
            StringBuilder _builder = new StringBuilder();
            
            do
            {
                _builder.Append(value[start]);
            } while (Move(1, ref start, count) && (_builder.Length < 2 || (value[start].Equals('"') && value[start - 1].Equals('\\') && !value[start - 2].Equals('\\'))));

            count -= _builder.Length;

            return _builder.ToString();
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
