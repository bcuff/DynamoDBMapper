using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Tests
{
    public class EnumMappingTests
    {
        [Theory]
        [InlineData(MockEnum.Zero, "0")]
        [InlineData(MockEnum.One, "1")]
        [InlineData(MockEnum.NegativeOne, "-1")]
        [InlineData(MockEnum.MinValue, "-2147483648")]
        [InlineData(MockEnum.MaxValue, "2147483647")]
        public void Enum_should_serialize_to_underlying_numerical_value(MockEnum value, string expectedNValue)
        {
            var doc = new MockDocument { Enum = value };
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(expectedNValue, attributes["Enum"].N);
        }

        [Theory]
        [InlineData(MockEnum.Zero, "0")]
        [InlineData(MockEnum.One, "1")]
        [InlineData(MockEnum.NegativeOne, "-1")]
        [InlineData(MockEnum.MinValue, "-2147483648")]
        [InlineData(MockEnum.MaxValue, "2147483647")]
        public void Enum_should_deserialize_from_underlying_numerical_value(MockEnum expectedValue, string attributeValueN)
        {
            var attributes = new Dictionary<string, AttributeValue> { { "Enum", new AttributeValue { N = attributeValueN } } };
            var doc = DocumentMapper.Default.ToDocument<MockDocument>(attributes);
            Assert.Equal(expectedValue, doc.Enum);
        }

        [Theory]
        [InlineData(MockEnum.Zero, "Zero")]
        [InlineData(MockEnum.One, "One")]
        [InlineData(MockEnum.NegativeOne, "NegativeOne")]
        [InlineData(MockEnum.MinValue, "MinValue")]
        [InlineData(MockEnum.MaxValue, "MaxValue")]
        public void Enum_should_deserialize_from_string_values(MockEnum expectedValue, string attributeValueS)
        {
            var attributes = new Dictionary<string, AttributeValue> { { "Enum", new AttributeValue { S = attributeValueS } } };
            var doc = DocumentMapper.Default.ToDocument<MockDocument>(attributes);
            Assert.Equal(expectedValue, doc.Enum);
        }
    }
}
