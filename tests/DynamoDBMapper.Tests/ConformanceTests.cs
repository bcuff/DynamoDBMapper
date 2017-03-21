using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DocumentModel;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public static class ConformanceTests
    {
        [Fact]
        public static void Empty_strings_should_not_map()
        {
            var attributes = DocumentMapper.Default.ToAttributes(new MockGenericDocument<string> { Value = "" });
            Assert.False(attributes.ContainsKey(nameof(MockGenericDocument<string>.Value)));
        }

        [Fact]
        public static void Empty_string_primitives_should_not_map()
        {
            var attributes = DocumentMapper.Default.ToAttributes(new MockGenericDocument<Primitive> { Value = new Primitive("") });
            Assert.False(attributes.ContainsKey(nameof(MockGenericDocument<Primitive>.Value)));
        }


        [Fact]
        public static void Empty_string_numeric_primitives_should_not_map()
        {
            var attributes = DocumentMapper.Default.ToAttributes(new MockGenericDocument<Primitive> { Value = new Primitive("", true) });
            Assert.False(attributes.ContainsKey(nameof(MockGenericDocument<Primitive>.Value)));
        }
    }
}
