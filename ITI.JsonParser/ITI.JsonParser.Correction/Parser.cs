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
/*        
        static private Double ParseDouble() {
            int endOfDoublePosition =
                findEndOfValuePosition( _json.Substring( _position ) );
            if( endOfDoublePosition == -1 ) {
                throw new FormatException( "Format Error: one of ,]} is expected" );
            }
            String stringValue = _json.Substring( _position, endOfDoublePosition );
            stringValue = stringValue.Replace('.',',');
            _position += endOfDoublePosition-1;
            if( double.TryParse( stringValue, out double doubleValue ) ) {
                return doubleValue;
            }
            throw new FormatException( "Format Error: double value is expected" );
        }

        static private String ParseString() {
            int closingDoubleQuotesPosition =
                findClosingDoubleQuotesPosition( _json.Substring( _position+1 ) );
            if( closingDoubleQuotesPosition == -1) {
                throw new FormatException( "Format Error: \" is expected" );
            }
            String stringValue = _json.Substring( _position+1, closingDoubleQuotesPosition);
            // TODO treat \u four-hex-digits number
            var myRegex = new Regex( @"[^\u0000-\uFFFF]" );

            _position += closingDoubleQuotesPosition + 1;
            return stringValue;
        }

        static private Boolean ParseBoolean() {
            int endOfBooleanPosition =
               findEndOfValuePosition( _json.Substring( _position ) );
            if( endOfBooleanPosition == -1 ) {
                throw new FormatException( "Format Error: one of ,]} is expected" );
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

        static private Object ParseNull( String json, ref int start, ref int count) {
            int endOfNullPosition =
               findEndOfValuePosition( _json.Substring( _position ) );
            if( endOfNullPosition == -1 ) {
                throw new FormatException( "Format Error: one of ,]} is expected" );
            }
            String stringValue = _json.Substring( _position, endOfNullPosition );
            _position += endOfNullPosition - 1;
            if( stringValue.Equals("null")) {
                return null;
            }
           throw new FormatException( "Format Error: invalid json value" );
        }

        static private List<Object> ParseArray( String json, ref int start, ref int count ) {
            object value = null;
            List<Object> anArray = new List<Object>();
            Stack<Char> arrDelimitors = new Stack<Char>();

            verifyEndOfJson(count);
            if( json[start + 1].Equals( ']' ) ) {
                increaseStart(ref start, ref count);
                return anArray;
            }
            // add '[' to stack
            increaseStart( ref start, ref count );
            arrDelimitors.Push( json[start] );
            var endOfArray = false;
            while( !endOfArray ) {
                //value = parseValue(); TODO
                anArray.Add( value );
                // end of the array
                increaseStart( ref start, ref count );

                if( !json[start].Equals( ',' ) ) {
                    if( arrDelimitors.Pop().Equals( '[' ) && !json[start].Equals( ']' ) ) {
                        throw new FormatException( "Format Error, ] is expected" );
                    }
                    endOfArray = true;
                } else {
                    increaseStart( ref start, ref count );
                }
            }
            return anArray;
        }
/*
        /**
         * finds the position of the first not escaped double commas 
         * */
        static private int findClosingDoubleQuotesPosition( String chain) {
            var myRegex = new Regex( "(?<!\\\\)\"" );
            foreach( Match match in myRegex.Matches( chain ) ) {
                return match.Index;
            }
            return -1;
        }

        /**
         * finds the position of one of the possible value endings
         * */
        static private int findEndOfValuePosition( String chain ) {
            char ch;
            for( int i=0; i<chain.Length; i++ ) {
                ch = chain[i];
                if( ch.Equals( ',' ) || ch.Equals( ']' ) || ch.Equals( '}' ) ) {
                    return i;
                }
            }
            return -1;
        }
        
        static private object parseValue(String json, ref int start, ref int count) {
            if(json[start].Equals('"')) {
                return ParseString( json, ref start, ref count );
            }
           if( json[start].Equals('[')) {
                return ParseArray( json, ref start, ref count );
            }
            if( json[start].Equals('{') ) {
                return ParseObject( json, ref start, ref count );
            }
            if( json[start].Equals('-') || int.TryParse( new String(json[start], 1), out int a ) ) { 
                return ParseDouble( json, ref start, ref count );
            }
            if( json[start].Equals('n') ) {
                return ParseNull( json, ref start, ref count );
            }
            if( json[start].Equals('f') || json[start].Equals( 't' ) ) {
                return ParseBoolean( json, ref start, ref count );
            }
            throw new FormatException( "Format Error: invalid json value" );
        }

        /**
         * JSon Object always begins with '{' and ends with '}'
         * 
         */
        static private Dictionary<String, Object> ParseObject(String json, ref int start, ref int count) {
            string key = null;
            object value = null;
            Dictionary<String, Object> anObject = new Dictionary<string, object>();
            Stack<Char> objDelimitors = new Stack<Char>();
  
            if( !json[start].Equals( '{' ) ) {
                throw new FormatException( @"Format Error, { is expected" );
            }

            verifyEndOfJson( count );
            if( json[start + 1].Equals( '}' ) ) {
                increaseStart( ref start, ref count );
                return anObject;
            }
            //add '{' to stack
            objDelimitors.Push( json[start] );
            increaseStart( ref start, ref count );

            var endOfObject = false;
            while( endOfObject ) {
                if( !json[start].Equals( '"' ) ) {
                    throw new FormatException( "Format Error, \" is expected" );
                }
                //key = ParseString();  TODO

                increaseStart( ref start, ref count );
                if( !json[start].Equals( ':' ) ) {
                    throw new FormatException( "Format Error, : is expected" );
                }
                increaseStart( ref start, ref count );

                //value = parseValue();  TODO
                if( anObject.ContainsKey( key ) ) {
                    throw new DuplicateKeyException( "key already exists" );
                }
                anObject.Add( key, value );

                // set cursor to the first character after the value
                increaseStart( ref start, ref count );

                if( json[start].Equals( ',' ) ) {
                    increaseStart( ref start, ref count );        // set cursor position after the comma
                }
                // end of object
                else if( json[start].Equals( '}' ) && objDelimitors.Pop().Equals( '{' ) ) {
                    endOfObject = true;
                } else {
                    throw new FormatException( "Format Error, of one '} is expected" );
                }
            }
            // TODO objectDelimitor must be empty
            return anObject;
        }

        static private void increaseStart( ref int start, ref int count ) {
            if( ( --count ) > 0 ) {
                ++start;
            } else {
                throw new FormatException( "Format Error, unexpected end of json chain" );
            }
        }
        static private void verifyEndOfJson( int count ) {
            if( ( count - 1 ) <= 0 ) {
                throw new FormatException( "Format Error, unexpected end of json chain" );
            }
        }


        static public Dictionary<String, Object> parse(String json, int start, int count) {
            json = Regex.Replace( json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1" );
            return ParseObject(json, ref start, ref count);
        }
    }
}
