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
        static Regex _regex = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);

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

        static char SkipSpaces(string value, ref int start, ref int count)
        {
            while (MoveNext(ref start, ref count) && start < value.Length && value[start].ToString().Trim().Length == 0)
            {
                //Do nothing
            };

            return value[start];
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
            return Decoder(FindStringOfString(value, ref start, ref count));
        }
        
        static string Decoder(string value)
        {
            return _regex.Replace(
                value,
                m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
            );
        }

        public static object[] ParseArray(string value, ref int start, ref int count)
        {
            List<object> _results = new List<object>();
            char _current_char;

            for (;;)
            {
                _current_char = SkipSpaces(value, ref start, ref count);
                _results.Add(ParseValue(_current_char, value, ref start, ref count));
                _current_char = SkipSpaces(value, ref start, ref count);

                if (']'.Equals(_current_char))
                {
                    break;
                }
                else if (!','.Equals(_current_char))
                {
                    throw new FormatException();
                }
            }

            return _results.ToArray();
        }

        public static Dictionary<string, object> ParseObject(string value, ref int start, ref int count)
        {
            if (value[start].ToString().Trim().Length == 0)
            {
                SkipSpaces(value, ref start, ref count);
            }

            if (!'{'.Equals(value[start]))
            {
                throw new FormatException();
            }

            Dictionary<string, object> _results = new Dictionary<string, object>();
            char _current_char;
            string _key;

            for(;;)
            {
                _current_char = SkipSpaces(value, ref start, ref count);

                if ('}'.Equals(_current_char))
                {
                    break;
                }
                else if (!'"'.Equals(_current_char))
                {
                    throw new FormatException();
                }

                _key = FindStringOfString(value, ref start, ref count);

                if (_results.ContainsKey(_key))
                {
                    throw new InvalidOperationException();
                }

                if (!':'.Equals(SkipSpaces(value, ref start, ref count)))
                {
                    throw new FormatException();
                }

                _current_char = SkipSpaces(value, ref start, ref count);
                _results.Add(_key, ParseValue(_current_char, value, ref start, ref count));
                _current_char = SkipSpaces(value, ref start, ref count);

                if ('}'.Equals(_current_char))
                {
                    break;
                }
                else if (!','.Equals(_current_char))
                {
                    throw new FormatException();
                }
            }

            return _results;
        }

        static object ParseValue(char ch, string value, ref int start, ref int count)
        {
            if ('"'.Equals(ch))
            {
                return ParseString(value, ref start, ref count);
            }
            else if ('n'.Equals(ch))
            {
                return ParseNull(value, ref start, ref count);
            }
            else if ('t'.Equals(ch) || 'f'.Equals(ch))
            {
                return ParseBoolean(value, ref start, ref count);
            }
            else if ('-'.Equals(ch) || Int32.TryParse(ch.ToString(), out int _result))
            {
                return ParseDouble(value, ref start, ref count);
            }
            else if ('['.Equals(ch))
            {
                return ParseArray(value, ref start, ref count);
            }
            else if ('{'.Equals(ch))
            {
                return ParseObject(value, ref start, ref count);
            }
            else
            {
                throw new FormatException();
            }
        }
    }
}
