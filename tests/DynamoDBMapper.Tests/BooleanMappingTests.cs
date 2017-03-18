using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public class BooleanMappingTests
    {
        public static TheoryData<AttributeValue, bool> BooleanData { get; } = new TheoryData<AttributeValue, bool>
        {
            { new AttributeValue { BOOL = true }, true },
            { new AttributeValue { BOOL = false }, false },
            { new AttributeValue { N = "0" }, false },
            { new AttributeValue { N = "1" }, true },
            { new AttributeValue { N = "-123" }, true },
            { null, false },
        };

        static DocumentMapper Mapper => DocumentMapper.Default;

        [Theory]
        [MemberData(nameof(BooleanData))]
        public static void Boolean_fields_should_deserialize_with_expected_values(AttributeValue attribute, bool expectedBooleanValue)
        {
            var before = new Dictionary<string, AttributeValue>();
            if (attribute != null)
            {
                before.Add("Boolean", attribute);
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
    }
}
