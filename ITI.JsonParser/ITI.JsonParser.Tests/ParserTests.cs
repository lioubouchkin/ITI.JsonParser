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
            int count = value.Length;
            bool _result = Parser.ParseBoolean(value, ref start, ref count);
            Assert.AreEqual(true, _result);
        }

        [TestCase(@"12.99]")]
        public void test_02(string value)
        {
            int start = 0;
            int count = value.Length;
            double _result = Parser.ParseDouble(value, ref start, ref count);
            Assert.AreEqual(12.99d, _result);
        }
    }
}
