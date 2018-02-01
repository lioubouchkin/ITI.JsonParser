using ITI.JsonParser.Correction;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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
    }
}
