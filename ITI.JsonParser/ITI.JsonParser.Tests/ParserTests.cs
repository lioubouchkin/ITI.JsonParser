using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using ITI.JsonParser.Correction;

namespace ITI.JsonParser.Tests {
    [TestFixture]
    public class ParserTests {
        [TestCase( @"true," )]
        public void test10_parse_boolean( string value ) {
            int start = 0;
            int count = value.Length - 1;
            bool actual = true;
            actual.Should().Be( Parser.ParseBoolean( value, ref start, ref count ) );
        }

        [TestCase( @"12.99]" )]
        public void test20_parse_double( string value ) {
            int start = 0;
            int count = value.Length - 1;
            double actual = 12.99d;
            actual.Should().Be( Parser.ParseDouble( value, ref start, ref count ) );
        }

        [TestCase( @"""""" )]
        public void test30_parse_empty_string( string value ) {
            int start = 0;
            int count = value.Length - 1;
            string actual = "";
            actual.Should().Be( Parser.ParseString( value, ref start, ref count ) );
        }

        [TestCase( @"""hello""" )]
        public void test31_parse_string( string value ) {
            int start = 0;
            int count = value.Length - 1;
            string actual = "hello";
            actual.Should().Be( Parser.ParseString( value, ref start, ref count ) );
        }

        //[TestCase( @"""William Shakespeare: \""To be, or not to be\""""" )]
        [TestCase( @"""W\""T\""""" )]
        public void test32_parse_escaped_string( string value ) {
            int start = 0;
            int count = value.Length - 1;
            string actual = "W\"T\"";
            //string actual = "William Shakespeare: \"To be, or not to be\"";
            actual.Should().Be( Parser.ParseString( value, ref start, ref count ) );
        }

        [TestCase( @"{}" )]
        public void test40_parse_empty_object( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> actual = Parser.ParseObject( value, ref start, ref count );
            actual.Should().NotBeNull();
        }

        [TestCase( @"   {    }   " )]
        public void test41_parse_empty_object_with_spaces( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> actual = Parser.ParseObject( value, ref start, ref count );
            actual.Should().NotBeNull();
        }

        [TestCase( @"{""  }   " )]
        public void test42_parse_invalid_object( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseObject( value, ref start, ref count );
            action.Should().Throw<FormatException>();
        }

        [TestCase( @"{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be\u002C or not to be\""""}" )]
        public void test43_parse_object_with_values( string value ) {
            int start = 0;
            int count = value.Length - 1;
            int actual = 4;
            actual.Should().Be( Parser.ParseObject( value, ref start, ref count ).Count );
        }

        [TestCase( @"{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}}" )]
        public void test44_parse_nested_object( string value ) {
            int start = 0;
            int count = value.Length - 1;
            int actual = 5;
            actual.Should().Be( Parser.ParseObject( value, ref start, ref count ).Count );
        }

        [TestCase( @"[""one"", 2.2, true, null, """", {""a"" : false}]" )]
        public void test50_parse_array( string value ) {
            int start = 0;
            int count = value.Length - 1;
            object[] actual = Parser.ParseArray( value, ref start, ref count );
            actual.Should().HaveCount(6);
        }

        [TestCase( @"{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}, ""weekend"" : [{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}},{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}}]}" )]
        public void test60_parse_complex_json( string value ) {
            int start = 0;
            int count = value.Length - 1;
            int actual = 6;
            actual.Should().Be( Parser.ParseObject( value, ref start, ref count ).Count );
        }
    }
}
