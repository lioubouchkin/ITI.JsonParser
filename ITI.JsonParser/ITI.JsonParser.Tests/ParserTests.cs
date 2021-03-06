﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using ITI.JsonParser.Correction;
using NUnit.Framework;

namespace ITI.JsonParser.Tests
{
    [TestFixture]
    public class ParserTests {

        [TestCase( @"{}" )]
        public void test10_empty_json( String jsonValue ) {
            Dictionary<String, Object> result = Parser.parse( jsonValue, 0, jsonValue.Length );
            Assert.AreEqual( result.Count, 0 );
        }

        [TestCase( @"adf   {   }   afd" )]
        public void test11_empty_json( String jsonValue ) {
            String json = @"   {   }   ";
            Dictionary<String, Object> result = Parser.parse( jsonValue, 3, json.Length );
            Assert.AreEqual( result.Count, 0 );
        }

        [TestCase( @"aze{""var"":""value""}aze" )]
        public void test20_one_key_value( String jsonValue ) {
            String json = @"{""var"":""value""}";
            Dictionary<String, Object> result = Parser.parse( jsonValue, 3, json.Length );
            Assert.AreEqual( result.Count, 1 );
        }

        [TestCase( @"{""var"":""value""" )]
        public void test21_unclosed_object_error( String jsonValue ) {
            String json = @"{""var"":""value""";
            Assert.Throws<FormatException>( delegate {
                Parser.parse( jsonValue, 0, json.Length );
            } );
        }

        [TestCase( @"aze  { ""var"" : ""value"" }  aze" )]
        public void test22_one_key_value_with_spaces( String jsonValue ) {
            String json = @"  { ""var"" : ""value"" }  ";
            Dictionary<String, Object> result = Parser.parse( jsonValue, 3, json.Length );
            Assert.AreEqual( result.Count, 1 );
        }
        /*
                [TestCase( @"{""var1"":""value1"",""var2"":""value2"",""var3"":""value3""}" )]
                public void test30_several_key_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 3 );
                }

                [TestCase( @"{""var1"":""value1"",""var2"":""value2"",""var1"":""value3""}" )]
                public void test40_duplicate_key_error( String jsonValue ) {
                    Assert.Throws<DuplicateKeyException>( delegate {
                        Parser.parse( jsonValue );
                    } );
                }

                [TestCase( @"{""var1"":25}" )]
                public void test50_double_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 1 );
                }

                [TestCase( @"{""var1"":25.5,""var2"":""value2"",""var3"":""12.88""}" )]
                public void test51_double_string_values( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 3 );
                }

                [TestCase( @"{""var1"":25.5,""var2"":""value2"",""var3"":null}" )]
                public void test60_null_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 3 );
                }

                [TestCase( @"{""var1"":25.5,""var2"":{""var1"":25},""var3"":null}" )]
                public void test70_object_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 3 );
                }

                [TestCase( @"{""var1"":25.5,""var2"":{""var1"":25},""var3"":{},""var4"":null}" )]
                public void test71_empty_object_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 4 );
                }

                [TestCase( @"{""var1"":25.5,""var2"":{""var1"":25,""var2"":true},""var3"":null,""var4"":false}" )]
                public void test80_boolean_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 4 );
                }

                [TestCase( @"{""var1"":[]}" )]
                public void test90_empty_array_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 1 );
                }

                [TestCase( @"{""var1"":25.5,""var2"":{""var1"":25,""var2"":true},""var3"":null,""var4"":false,""var5"":[],""var6"":[true,25.8,false,null,[]]}" )]
                public void test91_array_value( String jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 6 );
                }

                [TestCase( @"{""sentence"":""William Shakespeare : \""To be, or not to be\""""}" )]

                public void test100_parse_string_escape( string jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 1 );
                }

                [TestCase( @"{""weekend"":[""saturday"",""sunday""]}" )]
                public void test101_parse_array( string jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 1 );
                }

                [TestCase( @"{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}" )]
                public void test102_parse_object_simple( string jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 4 );
                }

                [TestCase( @"{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\"""",""weekend"":[""saturday"",""sunday""]}" )]
                public void test103_parse_object_with_array( string jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 5 );
                }

                //[TestCase( @"{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\"""",""weekend"":[""saturday"",""sunday""],""self"":{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare: \""To be, or not to be\"""",""weekend"":[""saturday"",""sunday""]},""complex"":[{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare: \""To be, or not to be\"""",""weekend"":[""saturday"",""sunday""]},{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare: \""To be, or not to be\"""",""weekend"":[""saturday"",""sunday""],""self"":{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare: \""To be, or not to be\"""",""weekend"":[""saturday"",""sunday""]}}]}" )]
                [TestCase( @"{  ""active"" : true,
                                ""age"" : 20,
                                ""salutation"" : ""hello"",
                                ""sentence""  :  ""William Shakespeare : \""To be, or not to be\"""",
                                ""weekend"" : [""saturday"",""sunday""],
                                ""self"" : 
                                    {   ""active"":true,
                                        ""age"":20,
                                        ""salutation"":""hello"",
                                        ""sentence"":""William Shakespeare: \""To be, or not to be\"""",
                                        ""weekend"":[""saturday"",""sunday""]
                                    }
                                    ,""complex"":
                                        [
                                            {
                                                ""active"":true,
                                                ""age"":20,
                                                ""salutation"":""hello"",
                                                ""sentence"":""William Shakespeare: \""To be, or not to be\"""",
                                                ""weekend"":
                                                    [
                                                        ""saturday"",""sunday""
                                                    ]
                                            }
                                            ,{
                                                ""active"":true,
                                                ""age"":20,
                                                ""salutation"":""hello"",
                                                ""sentence"":""William Shakespeare: \""To be, or not to be\"""",
                                                ""weekend"":
                                                    [
                                                        ""saturday"",""sunday""
                                                    ],
                                                ""self"":
                                                    {
                                                        ""active"":true,
                                                        ""age"":20,
                                                        ""salutation"":""hello"",
                                                        ""sentence"":""William Shakespeare: \""To be, or not to be\"""",
                                                        ""weekend"":
                                                            [
                                                                ""saturday"",
                                                                ""sunday""
                                                            ]
                                                    }
                                                }
                                            ]
                                        }" )]
                public void test104_parse_object_complex( string jsonValue ) {
                    Dictionary<String, Object> json = Parser.parse( jsonValue );
                    Assert.AreEqual( json.Count, 7 );
                }
          */
    }
}
