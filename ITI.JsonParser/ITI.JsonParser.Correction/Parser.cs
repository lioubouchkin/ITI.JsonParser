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

        static string FindStringOfValue(string value, ref int start, ref int count)
        {
            StringBuilder _builder = new StringBuilder();
            char _next;

            do
            {
                _builder.Append(value[start]);
                _next = start < value.Length - 1 ? value[start + 1] : '\0';
            } while (MoveNext(ref start, ref count) && !_next.Equals(',') && !_next.Equals(']') && !_next.Equals('}'));

            if (_next.Equals(',') || _next.Equals(']') || _next.Equals('}'))
            {
                MoveBack(ref start, ref count);
            }

            return _builder.ToString();
        }

        private static bool MoveBack(ref int start, ref int count)
        {
            return Move(ref start, -1, ref count);
        }

        private static bool MoveNext(ref int start, ref int count)
        {
            return Move(ref start, 1, ref count);
        }

        private static bool Move(ref int start, int step, ref int count)
        {
            if (step > count)
            {
                return false;
            }

            start += step;
            count -= step;

            return true;
        }

        static string FindStringOfString(string value, ref int start, ref int count)
        {
            StringBuilder _builder = new StringBuilder();
            char _current, _next;

            while (MoveNext(ref start, ref count))
            {
                _current = value[start];

                if (start == value.Length - 1)
                {
                    if (!_current.Equals('"'))
                    {
                        throw new FormatException();
                    }

                    break;
                }
                else
                {
                    _builder.Append(_current);
                    _next = value[start + 1];

                    if (_next.Equals('"') && !_current.Equals('\\'))
                    {
                        MoveNext(ref start, ref count);
                        break;
                    }
                }
            }

            return _builder.ToString();
        }

        static void SkipSpaces(string value, ref int start, ref int count)
        {
            while (MoveNext(ref start, ref count) && value[start].ToString().Trim().Length == 0)
            {
                //Do nothing
            }
        }

        public static object ParseNull(string value, ref int start, ref int count)
        {
            if (!"null".Equals(FindStringOfValue(value, ref start, ref count)))
            {
                throw new FormatException();
            }

            return null; 
        }

        public static bool ParseBoolean(string value, ref int start, ref int count)
        {
            return Boolean.Parse(FindStringOfValue(value, ref start, ref count));
        }

        public static double ParseDouble(string value, ref int start, ref int count)
        {
            return Double.Parse(FindStringOfValue(value, ref start, ref count), NumberStyles.Number, _culture);
        }

        public static string ParseString(string value, ref int start, ref int count)
        {
            return FindStringOfString(value, ref start, ref count);
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
