using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public class Int32MappingTests
    {
        static DocumentMapper Mapper => DocumentMapper.Default;

        public static TheoryData<int> Int32Data = new TheoryData<int>
        {
            0, 1, 234, int.MinValue, int.MaxValue
        };

        [Theory]
        [MemberData(nameof(Int32Data))]
        public static void Int32_fields_should_write_expected_output(int value)
        {
            var doc = new MockDocument { Int32 = value };
            var attributes = Mapper.ToAttributes(doc);
            Assert.Equal(value.ToString(), attributes["Int32"].N);
        }

        [Theory]
        [MemberData(nameof(Int32Data))]
        public static void Int32_N_fields_should_read_as_expected(int value)
        {
            var stringValue = value.ToString();
            var attributes = new Dictionary<string, AttributeValue> { ["Int32"] = new AttributeValue { N = value.ToString() } };
            var doc = Mapper.ToDocument<MockDocument>(attributes);
            Assert.Equal(value, doc.Int32);
        }

        [Theory]
        [MemberData(nameof(Int32Data))]
        public static void Int32_S_fields_should_read_as_expected(int value)
        {
            var stringValue = value.ToString();
            var attributes = new Dictionary<string, AttributeValue> { ["Int32"] = new AttributeValue { S = value.ToString() } };
            var doc = Mapper.ToDocument<MockDocument>(attributes);
            Assert.Equal(value, doc.Int32);
        }
    }
}
