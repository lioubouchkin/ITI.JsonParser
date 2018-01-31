using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.JsonParser
{
    public class Parser {

        StringBuilder _json;

        public Parser(String json) {
            _json = new StringBuilder( json );
        }

        double ParseDouble( String value ) {
            return NotImplementedException;
        }

        String ParseString( String value ) {
            return NotImplementedException;
        }

        bool ParseBoolean( String value ) {
            return NotImplementedException;
        }

        Object[] ParseArray( String value ) {
            return NotImplementedException;
        }

        Dictionary<String, Object> ParseObject() {
            return NotImplementedException;
        }


    }
}
