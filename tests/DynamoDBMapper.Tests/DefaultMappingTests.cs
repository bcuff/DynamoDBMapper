using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public class DefaultMappingTests
    {
        public static TheoryData<AttributeValue, bool> BooleanData { get; } = new TheoryData<AttributeValue, bool>
        {
            { new AttributeValue { BOOL = true }, true },
            { new AttributeValue { BOOL = false }, false },
            { new AttributeValue { N = "0" }, false },
            { new AttributeValue { N = "1" }, true },
            { new AttributeValue { N = "-123" }, true },
        };

        static DocumentMapper Mapper => DocumentMapper.Default;

        [Theory]
        [MemberData(nameof(BooleanData))]
        public static void Boolean_fields_should_deserialize_with_expected_values(AttributeValue attribute, bool expectedBooleanValue)
        {
            var before = new Dictionary<string, AttributeValue>
            {
                { "Boolean", attribute }
            };
            var doc = Mapper.ToDocument<MockDocument>(before);
            Assert.Equal(expectedBooleanValue, doc.Boolean);
        }

        [Theory]
        [InlineData(false, "0")]
        [InlineData(true, "1")]
        public static void Boolean_fields_should_serialize_with_expected_values(bool value, string expectedN)
        {
            var before = new MockDocument { Boolean = value };
            var after = Mapper.ToAttributes(before);
            Assert.Equal(expectedN, after["Boolean"].N);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("the quick brown fox...")]
        public static void String_fields_should_map_with_expected_values(string value)
        {
            var doc = new MockDocument { String = value };
            var attributes = Mapper.ToAttributes(doc);
            Assert.Equal(value, attributes["String"].S);
        }

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
