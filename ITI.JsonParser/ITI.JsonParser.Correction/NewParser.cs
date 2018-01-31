using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITI.JsonParser.Correction {
    public static class NewParser {
        private static Dictionary<String, Object> _result;
        private static String _json;
        // current parsing element position
        private static int _position;
        // objects / vakues delimitors
        private static Stack<Char> _delimitors;

        /*       public Parser( String json ) {
                   //_json = json.Trim();
                   _json = Regex.Replace( json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1" );
                   _position = 0;
                   _result = new Dictionary<String, Object>();
               }
       */
        private static Double ParseDouble( string value, int start, int count ) {

            String stringValue = _json.Substring( _position, start + count );
            stringValue = stringValue.Replace( '.', ',' );
            _position += start + count - 1;
            if( double.TryParse( stringValue, out double doubleValue ) ) {
                return doubleValue;
            }
            throw new FormatException( "Format Error: double value is expected" );
        }

        private static String ParseString( string value, int start, int count ) {
            String stringValue = _json.Substring( _position + 1, start + count );
            _position += start + count + 1;
            return stringValue;
        }

        private static Boolean ParseBoolean( string value, int start, int count ) {
            String stringValue = _json.Substring( _position, start + count );
            _position += start + count - 1;
            if( stringValue.Equals( "true" ) ) {
                return true;
            }
            if( stringValue.Equals( "false" ) ) {
                return false;
            }
            throw new FormatException( "Format Error: invalid json value" );
        }

        private static Object ParseNull( string value, int start, int count ) {
            String stringValue = _json.Substring( _position, start + count );
            _position += start + count - 1;
            if( stringValue.Equals( "null" ) ) {
                return null;
            }
            throw new FormatException( "Format Error: invalid json value" );
        }

        private static List<Object> ParseArray() {
            object value = null;
            List<Object> anArray = new List<Object>();
            try {
                if( _json[_position + 1].Equals( ']' ) ) {
                    _position++;
                    return anArray;
                }
                // add '[' to stack
                _delimitors.Push( _json[_position++] );
                var endOfArray = false;
                while( !endOfArray ) {

                    value = ParseValue();
                    anArray.Add( value );
                    // end of the array
                    if( !_json[++_position].Equals( ',' ) ) {
                        if( _delimitors.Pop().Equals( '[' ) && !_json[_position].Equals( ']' ) ) {
                            throw new FormatException( "Format Error, ] is expected" );
                        }
                        endOfArray = true;
                    } else {
                        ++_position;
                    }
                }
            } catch( IndexOutOfRangeException e ) {
                throw new FormatException( "Format Error: unexpected end of json" );
            }
            return anArray;
        }

        /**
         * finds the position of the first not escaped double commas 
         * */
        private static int FindClosingDoubleQuotesPosition( String chain ) {
            var myRegex = new Regex( "(?<!\\\\)\"" );
            foreach( Match match in myRegex.Matches( chain ) ) {
                return match.Index;
            }
            return -1;
        }

        /**
         * finds the position of one of the possible value endings
         * */
        private static int FindEndOfValuePosition( String chain ) {
            char ch;
            for( int i = 0; i < chain.Length; i++ ) {
                ch = chain[i];
                if( ch.Equals( ',' ) || ch.Equals( ']' ) || ch.Equals( '}' ) ) {
                    return i;
                }
            }
            return -1;
        }

        private static object ParseValue() {
            if( _json[_position].Equals( '"' ) ) {
                int closingDoubleQuotesPosition =
                    FindClosingDoubleQuotesPosition( _json.Substring( _position + 1 ) );
                if( closingDoubleQuotesPosition == -1 ) {
                    throw new FormatException( "Format Error: \" is expected" );
                }
                return ParseString( _json, _position, closingDoubleQuotesPosition - _position );
            }
            if( _json[_position].Equals( '[' ) ) {
                return ParseArray();
            }
            if( _json[_position].Equals( '{' ) ) {
                return ParseObject();
            }
            if( int.TryParse( new String( _json[_position], 1 ), out int a ) ) {
                int endOfDoublePosition =
                FindEndOfValuePosition( _json.Substring( _position ) );
                if( endOfDoublePosition == -1 ) {
                    throw new FormatException( "Format Error: one of ,]} is expected" );
                }
                return ParseDouble( _json, _position, endOfDoublePosition - _position );
            }
            if( _json[_position].Equals( 'n' ) ) {
                int endOfNullPosition =
                FindEndOfValuePosition( _json.Substring( _position ) );
                if( endOfNullPosition == -1 ) {
                    throw new FormatException( "Format Error: one of ,]} is expected" );
                }
                return ParseNull( _json, _position, endOfNullPosition - _position );
            }
            if( _json[_position].Equals( 'f' ) || _json[_position].Equals( 't' ) ) {
                int endOfBooleanPosition =
                FindEndOfValuePosition( _json.Substring( _position ) );
                if( endOfBooleanPosition == -1 ) {
                    throw new FormatException( "Format Error: one of ,]} is expected" );
                }
                return ParseBoolean( _json, _position, endOfBooleanPosition - _position );
            }
            throw new FormatException( "Format Error: invalid json value" );
        }

        /**
         * JSon Object always begins with '{' and ends with '}'
         * 
         */
        private static Dictionary<String, Object> ParseObject() {
            string key = null;
            object value = null;
            Dictionary<String, Object> anObject = new Dictionary<string, object>();
            try {
                if( _json[_position + 1].Equals( '}' ) ) {
                    _position++;
                    return anObject;
                }
                // add '{' to stack
                _delimitors.Push( _json[_position++] );
                var endOfObject = false;
                while( !endOfObject ) {
                    if( !_json[_position].Equals( '"' ) ) {
                        throw new FormatException( "Format Error, \" is expected" );
                    }
                    key = (String)ParseValue();

                    if( !_json[++_position].Equals( ':' ) ) {
                        throw new FormatException( "Format Error, : is expected" );
                    }
                    ++_position;
                    value = ParseValue();
                    if( anObject.ContainsKey( key ) ) {
                        throw new DuplicateKeyException( "key already exists" );
                    }
                    anObject.Add( key, value );

                    // end of the object
                    if( !_json[++_position].Equals( ',' ) ) {
                        if( _delimitors.Pop().Equals( '{' ) && !_json[_position].Equals( '}' ) ) {
                            throw new FormatException( "Format Error, } is expected" );
                        }
                        endOfObject = true;
                    } else {
                        ++_position;
                    }
                }
            } catch( IndexOutOfRangeException e ) {
                throw new FormatException( "Format Error: unexpected end of json string" );
            }
            return anObject;
        }


        public static Dictionary<String, Object> Parse( String json ) {
            _json = Regex.Replace( json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1" );
            _position = 0;
            _result = new Dictionary<String, Object>();
            _delimitors = new Stack<Char>();
            if( !_json[_position].Equals( '{' ) ) {
                throw new FormatException( @"Format Error, { is expected" );
            }
            if( _json.Substring( 1, _json.Length - 2 ).Trim().Length == 0 ) {
                if( !_json[++_position].Equals( '}' ) ) {
                    throw new FormatException( @"Format Error, } is expected" );
                }
                return _result;
            }
            _result = (Dictionary<String, Object>)ParseValue();

            if( _delimitors.Count > 0 ) {
                throw new FormatException( "Format Error" );
            }
            return _result;
        }
    }

}
