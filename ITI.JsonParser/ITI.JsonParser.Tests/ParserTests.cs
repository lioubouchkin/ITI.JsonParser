using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ITI.JsonParser;
namespace ITI.JsonParser.Tests
{
    [TestFixture]
    public class ParserTests {
        [TestCase( @"true," )]
        public void test_01( string value ) {
            int start = 0;
            int count = value.Length - 1;
            bool _result = Parser.ParseBoolean(value, ref start, ref count);
            Assert.AreEqual(true, _result);
        }

        [TestCase(@"12.99]")]
        public void test_02(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            double _result = Parser.ParseDouble(value, ref start, ref count);
            Assert.AreEqual(12.99d, _result);
        }

        [TestCase(@"""""")]
        public void test_03(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            string _result = Parser.ParseString(value, ref start, ref count);
            Assert.AreEqual("", _result);
        }

        [TestCase(@"""a""")]
        public void test_04(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            string _result = Parser.ParseString(value, ref start, ref count);
            Assert.AreEqual("a", _result);
        }

        [TestCase(@"""hello""")]
        public void test_05(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            string _result = Parser.ParseString(value, ref start, ref count);
            Assert.AreEqual("hello", _result);
        }

        [TestCase(@"""William Shakespeare: \""To be, or not to be\""""")]
        public void test_06(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            string _result = Parser.ParseString(value, ref start, ref count);
            Assert.AreEqual("William Shakespeare: \\\"To be, or not to be\\\"", _result);
        }

        [TestCase(@"{}")]
        public void test_07(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> _result = Parser.ParseObject(value, ref start, ref count);
            Assert.AreEqual(0, _result.Count);
        }

        [TestCase(@"   {    }   ")]
        public void test_08(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> _result = Parser.ParseObject(value, ref start, ref count);
            Assert.AreEqual(0, _result.Count);
        }

        [TestCase(@"{""  }   ")]
        public void test_09(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            Assert.Throws<FormatException>(delegate
           {
               Parser.ParseObject(value, ref start, ref count);
           });
        }

        [TestCase(@"{""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be\u002C or not to be\""""}")]
        public void test_10(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> _result = Parser.ParseObject(value, ref start, ref count);
            Assert.AreEqual(4, _result.Count);
        }

        [TestCase(@"{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}}")]
        public void test_11(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> _result = Parser.ParseObject(value, ref start, ref count);
            Assert.AreEqual(5, _result.Count);
        }

        [TestCase(@"{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}, ""weekend"" : [{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}},{""active"" : true       ,""age"" : 20    ,""salutation"" : ""hello"",""sentence"" : ""William Shakespeare : \""To be, or not to be\"""", ""self"": {""active"":true,""age"":20,""salutation"":""hello"",""sentence"":""William Shakespeare : \""To be, or not to be\""""}}]}")]
        public void test_12(string value)
        {
            int start = 0;
            int count = value.Length - 1;
            Dictionary<string, object> _result = Parser.ParseObject(value, ref start, ref count);
            Assert.AreEqual(6, _result.Count);
        }
    }
}
