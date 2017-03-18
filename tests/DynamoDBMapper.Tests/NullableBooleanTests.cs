using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public class NullableBooleanMappingTests
    {
        public static TheoryData<AttributeValue, bool?> BooleanData { get; } = new TheoryData<AttributeValue, bool?>
        {
            { new AttributeValue { BOOL = true }, true },
            { new AttributeValue { BOOL = false }, false },
            { new AttributeValue { N = "0" }, false },
            { new AttributeValue { N = "1" }, true },
            { new AttributeValue { N = "-123" }, true },
            { new AttributeValue { NULL = true }, default(bool?) },
            { null, default(bool?) },
        };

        static DocumentMapper Mapper => DocumentMapper.Default;

        [Theory]
        [MemberData(nameof(BooleanData))]
        public static void NullableBoolean_fields_should_deserialize_with_expected_values(AttributeValue attribute, bool? expectedBooleanValue)
        {
            var before = new Dictionary<string, AttributeValue>();
            if (attribute != null)
            {
                before.Add("NullableBoolean", attribute);
            };
            var doc = Mapper.ToDocument<MockDocument>(before);
            Assert.Equal(expectedBooleanValue, doc.NullableBoolean);
        }
    }
}
