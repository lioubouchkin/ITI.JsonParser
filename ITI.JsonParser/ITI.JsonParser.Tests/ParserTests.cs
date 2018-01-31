using System;
using System.Collections.Generic;
using System.Data.Linq;
using ITI.JsonParser.Correction;
using NUnit.Framework;

namespace ITI.JsonParser.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [TestCase(@"{}")]
        public void test10_empty_json(String jsonValue) {
            Parser parser = new Parser( jsonValue );
            //Assert.That (parser.findClosingDoubleQuotesPosition( "\\\"sdsd\""), Is.EqualTo(6) );
            Dictionary<String, Object> json = parser.parse();
            Assert.AreEqual( json.Count, 0);
        }

        [TestCase( @"{""var"":""value""}" )]
        //[TestCase( @"{""var"":""""}" )]
        public void test20_one_key_value( String jsonValue ) {
            Parser parser = new Parser( jsonValue );
            Dictionary<String, Object> json = parser.parse();
            Assert.AreEqual( json.Count, 1 );
        }

        [TestCase( @"{""var1"":""value1"",""var2"":""value2"",""var3"":""value3""}" )]
        //[TestCase( @"{""var1"":"""",""var2"":"""",""var3"":""""}" )]
        public void test30_several_key_value( String jsonValue ) {
            Parser parser = new Parser( jsonValue );
            Dictionary<String, Object> json = parser.parse();
            Assert.AreEqual( json.Count, 3 );
        }

        [TestCase( @"{""var1"":""value1"",""var2"":""value2"",""var1"":""value3""}" )]
        //[TestCase( @"{""var1"":"""",""var2"":"""",""var1"":""""}" )]
        public void test40_duplicate_key_error( String jsonValue ) {
            Parser parser = new Parser( jsonValue );
            //Dictionary<String, Object> json = parser.parse();
            Assert.Throws<DuplicateKeyException>( delegate {
                parser.parse();
            } );
        }

    }
}
