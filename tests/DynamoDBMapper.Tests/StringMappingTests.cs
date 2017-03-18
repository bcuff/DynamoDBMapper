using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public class StringMappingTests
    {
        static DocumentMapper Mapper => DocumentMapper.Default;

        [Theory]
        [InlineData("foo")]
        [InlineData("the quick brown fox...")]
        public static void String_fields_should_map_with_expected_values(string value)
        {
            var doc = new MockDocument { String = value };
            var attributes = Mapper.ToAttributes(doc);
            Assert.Equal(value, attributes["String"].S);
        }
    }
}
