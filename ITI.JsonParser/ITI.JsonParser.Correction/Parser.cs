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
                _next = start + 1 < value.Length ? value[start + 1] : '\0';
            } while (Move(ref start, 1, count) && !_next.Equals(',') && !_next.Equals(']') && !_next.Equals('}'));

            count -= _builder.Length;

            return _builder.ToString();
        }

        private static bool Move(ref int start, int step, int count)
        {
            if (step > count)
            {
                return false;
            }

            start += step;

            return true;
        }

        static string findStringOfString(string value, ref int start, ref int count)
        {
            StringBuilder _builder = new StringBuilder();
            char _current, _next;

            while (Move(ref start, 1, count))
            {
                _current = value[start];

                if (start + 1 == value.Length)
                {
                    if (!_current.Equals('"'))
                    {
                        _builder.Append(_current);
                    }

                    break;
                }
                else
                {
                    _builder.Append(_current);
                    _next = value[start + 1];

                    if (_next.Equals('"') && !_current.Equals('\\'))
                    {
                        break;
                    }
                }
            }

            if (count == 1)
            {
                count--;
            }
            else if (Move(ref start, 1, count))
            {
                count -= _builder.Length + 2;
            }

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
