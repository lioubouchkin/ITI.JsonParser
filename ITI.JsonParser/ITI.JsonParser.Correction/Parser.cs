using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 
using System.Threading.Tasks;

namespace ITI.JsonParser.Correction
{
    /**
     * Parser doesn't treat spaces actually
     * 
     * 
     */
    public class Parser {
        Dictionary<String, Object> _result;
        String _json;
        // current parsing element position
        int _position;
        // objects / vakues delimitors
        Stack<Char> _delimitors = new Stack<Char>();

        public Parser( String json ) {
            _json = json.Trim();
            _position = 0;
            _result = new Dictionary<String, Object>();
        }
        
        Double ParseDouble() {
            int endOfDoublePosition =
                findEndOfValuePosition( _json.Substring( _position ) );
            if( endOfDoublePosition == -1 ) {
                throw new FormatException( "Format Error: one of ,]} is unexpected" );
            }
            String stringValue = _json.Substring( _position, endOfDoublePosition );
            stringValue = stringValue.Replace('.',',');
            _position += endOfDoublePosition-1;
            if( double.TryParse( stringValue, out double doubleValue ) ) {
                return doubleValue;
            }
            throw new FormatException( "Format Error: double value is unexpected" );
        }

        String ParseString() {
            int closingDoubleQuotesPosition =
                findClosingDoubleQuotesPosition( _json.Substring( _position+1 ) );
            if( closingDoubleQuotesPosition == -1) {
                throw new FormatException( "Format Error: \" is unexpected" );
            }
            String stringValue = _json.Substring( _position+1, closingDoubleQuotesPosition);
            _position += closingDoubleQuotesPosition + 1;
            return stringValue;
        }
        
        Boolean ParseBoolean() {
            int endOfBooleanPosition =
               findEndOfValuePosition( _json.Substring( _position ) );
            if( endOfBooleanPosition == -1 ) {
                throw new FormatException( "Format Error: one of ,]} is unexpected" );
            }
            String stringValue = _json.Substring( _position, endOfBooleanPosition );
            _position += endOfBooleanPosition - 1;
            if( stringValue.Equals( "true" ) ) {
                return true;
            }
            if( stringValue.Equals( "false" ) ) {
                return false;
            }
            throw new FormatException( "Format Error: invalid json value" );
        }
        
        Object ParseNull() {
            int endOfNullPosition =
               findEndOfValuePosition( _json.Substring( _position ) );
            if( endOfNullPosition == -1 ) {
                throw new FormatException( "Format Error: one of ,]} is unexpected" );
            }
            String stringValue = _json.Substring( _position, endOfNullPosition );
            _position += endOfNullPosition - 1;
            if( stringValue.Equals("null")) {
                return null;
            }
            throw new FormatException( "Format Error: invalid json value" );
        }

        // TODO
        List<Object> ParseArray() {
            object value = null;
            List<Object> anArray = new List<Object>();
            try {
                if( _json[_position + 1].Equals( ']' ) ) {
                    _position++;
                    return anArray;
                }
                // add '[' to stack
                _delimitors.Push( _json[_position++] );
                var endOfArray = false;     // TODO treat end of array if blank space
                while( !endOfArray ) {

                    value = parseValue();
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
        public int findClosingDoubleQuotesPosition( String chain) {
            var myRegex = new Regex( "(?<!\\\\)\"" );
            foreach( Match match in myRegex.Matches( chain ) ) {
                return match.Index;
            }
            return -1;
        }

        /**
         * finds the position of the first not escaped double commas 
         * */
        public int findEndOfValuePosition( String chain ) {
            char ch;
            for( int i=0; i<chain.Length; i++ ) {
                ch = chain[i];
                if( ch.Equals( ',' ) || ch.Equals( ']' ) || ch.Equals( '}' ) ) {
                    return i;
                }
            }
            return -1;
        }

        // TODO
        object parseValue() {
            if(_json[_position].Equals('"')) {
                return ParseString();
            }
            if( _json[_position].Equals('[')) {
                return ParseArray();
            }
            if( _json[_position].Equals('{') ) {
                return parseObject();
            }
            if( int.TryParse( new String(_json[_position], 1), out int a ) ) {
                return ParseDouble();
            }
            if( _json[_position].Equals('n') ) {
                return ParseNull();
            }
            if( _json[_position].Equals('f') || _json[_position].Equals( 't' ) ) {
                return ParseBoolean();
            }
            throw new FormatException( "Format Error: invalid json value" );
        }

        /**
         * JSon Object always begins with '{' and ends with '}'
         * 
         */
        Dictionary<String, Object> parseObject() {
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
                    key = ParseString();

                    if( !_json[++_position].Equals( ':' ) ) {
                        throw new FormatException( "Format Error, : is expected" );
                    }
                    ++_position;
                    value = parseValue();       // TODO
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


        public Dictionary<String, Object> parse() {
            if ( !_json[_position].Equals('{') ) {
                throw new FormatException( @"Format Error, { is expected" );
            }
            if ( _json.Substring( 1, _json.Length - 2 ).Trim().Length == 0 ) {
                if( !_json[++_position].Equals('}') ) {
                    throw new FormatException( @"Format Error, } is expected" );
                }
                return _result;
            }
             _result = parseObject();

            if(_delimitors.Count > 0) {
                throw new FormatException( "Format Error" );
            }
            return _result;
        }
    }
}
