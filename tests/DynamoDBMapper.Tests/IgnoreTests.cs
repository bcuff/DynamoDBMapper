using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Tests
{
    public class IgnoreTests
    {
        [Fact]
        public void Mapper_respects_DynamoDBIgnore_attribute()
        {
            var doc = new MockDocument { IgnoredField = "123" };
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.False(attributes.ContainsKey("IgnoredField"));
            attributes = new Dictionary<string, AttributeValue> { { nameof(MockDocument.IgnoredField), new AttributeValue { S = "234" } } };
            doc = DocumentMapper.Default.ToDocument<MockDocument>(attributes);
            Assert.Equal(null, doc.IgnoredField);
        }
    }
}
