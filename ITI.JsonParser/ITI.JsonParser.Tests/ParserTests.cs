using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using ITI.JsonParser.Correction;

namespace ITI.JsonParser.Tests {
    [TestFixture]
    public class ParserTests {
        [TestCase( @"true" )]
        public void test10_parse_boolean( string value ) {
            int start = 0;
            int count = value.Length - 1;
            bool actual = true;
            actual.Should().Be( Parser.ParseBoolean( value, ref start, ref count ) );
        }

        [TestCase( @"truth" )]
        public void test11_parse_invalid_boolean( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseBoolean( value, ref start, ref count );
            action.Should().Throw<FormatException>();
        }

        [TestCase( @"12.99" )]
        public void test20_parse_double( string value ) {
            int start = 0;
            int count = value.Length - 1;
            double actual = 12.99d;
            actual.Should().Be( Parser.ParseDouble( value, ref start, ref count ) );
        }

        [TestCase( @"-12.99" )]
        public void test21_parse_negative_double( string value ) {
            int start = 0;
            int count = value.Length - 1;
            double actual = -12.99d;
            actual.Should().Be( Parser.ParseDouble( value, ref start, ref count ) );
        }

        [TestCase( @"-12.k9" )]
        public void test22_parse_invalid_double( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseDouble( value, ref start, ref count );
            action.Should().Throw<FormatException>();
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

        // not closed string value
        [TestCase( @"""hello" )]
        public void test32_parse_invalid_string( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseString( value, ref start, ref count );
            action.Should().Throw<FormatException>();
        }

        [TestCase( @"null" )]
        public void test40_parse_null( string value ) {
            int start = 0;
            int count = value.Length - 1;
            object actual = null;
            actual.Should().Be( Parser.ParseNull( value, ref start, ref count ) );
        }

        [TestCase( @"nul" )]
        public void test41_parse_invalid_null( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseNull( value, ref start, ref count );
            action.Should().Throw<FormatException>();
        }

        [TestCase( @"[]" )]
        public void test50_parse_empty_array( string value ) {
            int start = 0;
            int count = value.Length - 1;
            object[] actual = Parser.ParseArray( value, ref start, ref count );
            actual.Should().NotBeNull();
        }

        [TestCase( @"[""Elliot"",2.58,"""",false,[],null]" )]
        public void test51_parse_array_with_values( string value ) {
            int start = 0;
            int count = value.Length - 1;
            int actual = 6;
            actual.Should().Be( Parser.ParseArray( value, ref start, ref count ).Length );
        }

        // array can't contain pairs key:value
        [TestCase( @"[""Elliot"",2.58,"""",false,[],null,false:""error""]" )]
        public void test52_parse_invalid_array( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseArray( value, ref start, ref count );
            action.Should().Throw<FormatException>();
        }

        [TestCase( @"{}" )]
        public void test60_parse_empty_object( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> actual = Parser.ParseObject( value, ref start, ref count );
            actual.Should().NotBeNull();
        }

        [TestCase( @"{""Joseph"":""Heller""}" )]
        public void test61_parse_object_with_pair_value( string value ) {
            int start = 0;
            int count = value.Length - 1;
            int actual = 1;
            actual.Should().Be( Parser.ParseObject( value, ref start, ref count ).Count );
        }

        [TestCase( @"{""active"":true,""age"":20,""salutation"":""hello"",""array"":[""Elliot"",2.58,"""",false,[],null],""object"":{""Joseph"":""Heller""}}" )]
        public void test62_parse_object_with_differents_values( string value ) {
            int start = 0;
            int count = value.Length - 1;
            int actual = 5;
            actual.Should().Be( Parser.ParseObject( value, ref start, ref count ).Count );
        }

        [TestCase( @"{""active"":true,""age"":20,""active"":""false""}" )]
        public void test63_duplicate_key_in_object_error( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseObject( value, ref start, ref count );
            action.Should().Throw<InvalidOperationException>();
        }

        // invalid pair key:value in jason object
        [TestCase( @"{""active"":true,""age"",""active"":""false""}" )]
        public void test64_json_format_error( string value ) {
            int start = 0;
            int count = value.Length - 1;
            Action action = () => Parser.ParseObject( value, ref start, ref count );
            action.Should().Throw<FormatException>();
        }

        [TestCase( @"
            {   ""active"" : true       ,
                ""age"" : 20    ,
                ""salutation"" : ""hello"",  
                ""Null"" :   null  ,
                ""self"": 
                    {   ""active""  : true  ,
                        ""age"" :  20  ,  
                        ""salutation"":""hello""
                    }
            }" )]
        public void test70_parse_object_with_spaces( string value ) {
            int start = 0;
            int count = value.Length - 1;
            int actual = 5;
            actual.Should().Be( Parser.ParseObject( value, ref start, ref count ).Count );
        }

        [TestCase( @"""To be\u002C \u006Fr n\u006Ft t\u006F be""" )]
        public void test80_parse_string_with_hexadecimals( string value ) {
            int start = 0;
            int count = value.Length - 1;
            string actual = "To be, or not to be";
            actual.Should().BeEquivalentTo( Parser.ParseString( value, ref start, ref count ) );
        }

        [TestCase( @"""William Shakespeare : \""To be, or not to be\""""" )]
        // [TestCase( "\"William Shakespeare : \\\"To be, or not to be\\\"\"" )]    // equivalent
        public void test81_parse_escaped_string( string value ) {
            int start = 0;
            int count = value.Length - 1;
            string actual = "William Shakespeare : \"To be, or not to be\"";
            actual.Should().BeEquivalentTo( Parser.ParseString( value, ref start, ref count ) );
        }
    }
}
