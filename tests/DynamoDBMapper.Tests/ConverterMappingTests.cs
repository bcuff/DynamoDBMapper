using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Tests
{
    public class ConverterMappingTests
    {
        [Theory]
        [InlineData("foo", "oof")]
        [InlineData("this is a test", "tset a si siht")]
        public void ReversedString_field_should_map_to_expected_attribute_values(string before, string after)
        {
            var doc = new MockDocument { ReversedString = before };
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(after, attributes["ReversedString"].S);
        }

        [Theory]
        [InlineData("foo", "oof")]
        [InlineData("this is a test", "tset a si siht")]
        public void ReversedString_field_should_map_from_expected_attribute_values(string before, string after)
        {
            var attributes = new Dictionary<string, AttributeValue> { { nameof(MockDocument.ReversedString), new AttributeValue { S = before } } };
            var doc = DocumentMapper.Default.ToDocument<MockDocument>(attributes);
            Assert.Equal(after, doc.ReversedString);
        }
    }
}
