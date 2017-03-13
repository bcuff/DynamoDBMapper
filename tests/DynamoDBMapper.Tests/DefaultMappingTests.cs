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

        [Theory]
        [MemberData("BooleanData")]
        public static void Boolean_fields_should_map_with_expected_values(AttributeValue attribute, bool expectedBooleanValue)
        {
            var mapper = DocumentMapper.Default;
            var before = new Dictionary<string, AttributeValue>
            {
                { "Boolean", attribute }
            };
            var doc = mapper.ToDocument<MockDocument>(before);
            Assert.Equal(expectedBooleanValue, doc.Boolean);
        }
    }
}
