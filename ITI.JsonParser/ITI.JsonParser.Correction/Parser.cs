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

        double ParseDouble( String value ) {
            throw new NotImplementedException();
        }

        String ParseString() {
            int closingDoubleQuotesPosition =
                findClosingDoubleQuotesPosition( _json.Substring( _position+1 ) );
            String stringValue = _json.Substring( _position+1, closingDoubleQuotesPosition);
            _position += closingDoubleQuotesPosition+1;
            return stringValue;
        }

        bool ParseBoolean( String value ) {
            throw new NotImplementedException();
        }

        Object[] ParseArray( String value ) {
            return null;
            //throw new NotImplementedException();
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

        // TODO
        object parseValue() {
            if(_json[_position].Equals('"')) {
                return ParseString();
            }
            if( _json[_position].Equals('[') ) {
                return ParseArray();
            }
            return "aValue";
//            throw new NotImplementedException();
        }

        /**
         * JSon Object always begins with '{' and ends with '}'
         * 
         */
        Dictionary<String, Object> parseObject() {
            string key = null;
            object value = null;
            // add '{' to stack
            _delimitors.Push( _json[_position++] );
            Dictionary<String, Object> anObject = new Dictionary<string, object>();
            var endOfObject = false;
            while( !endOfObject ) {
                if( !_json[_position].Equals( '"' ) ) {
                    throw new FormatException( "Format Error, \" is needed" );
                }
                key = ParseString();    // TODO
                if( !_json[++_position].Equals( ':' ) ) {
                    throw new FormatException( "Format Error, : is needed" );
                }
                ++_position;
                value = parseValue();       // TODO
                if( anObject.ContainsKey( key ) ) {
                    throw new DuplicateKeyException( "key already exists" );
                }
                anObject.Add(key, value);

                // end of the object
                if( !_json[++_position].Equals(',') ) {
                    if( _delimitors.Pop().Equals('{') && !_json[_position].Equals('}') ) {
                        throw new FormatException( "Format Error, } is needed" );
                    }
                    endOfObject = true;
                }
                ++_position;
            }
            return anObject;
        }


        public Dictionary<String, Object> parse() {
            if ( !_json[_position].Equals('{') ) {
                throw new FormatException( @"Format Error, '{' is needed" );
            }
            if ( _json.Substring( 1, _json.Length - 2 ).Trim().Length == 0 ) {
                if( !_json[++_position].Equals('}') ) {
                    throw new FormatException( @"Format Error, '}' is needed" );
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
