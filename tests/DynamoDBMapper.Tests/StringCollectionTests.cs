using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public static class StringCollectionTests
    {
        public static TheoryData<IEnumerable<string>> StringValues = new TheoryData<IEnumerable<string>>
        {
            { new [] { "test", "the quick brown fox!" } },
            { new string[0] },
        };

        [Theory]
        [MemberData(nameof(StringValues))]
        public static void StringCollections_write_to_string_set_field(IEnumerable<string> values)
        {
            StringCollections_write_to_string_set_field<List<string>>(values);
            StringCollections_write_to_string_set_field<HashSet<string>>(values);
        }

        [Theory]
        [MemberData(nameof(StringValues))]
        public static void StringArrays_write_to_string_set_field(IEnumerable<string> values)
        {
            var doc = new MockDocumentWithStringCollection<string[]> { StringCollection = values.ToArray() };
            var expected = values.ToList();
           var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(expected, attributes["StringCollection"].SS);
        }

        public static void StringCollections_write_to_string_set_field<T>(IEnumerable<string> values)
            where T : ICollection<string>, new()
        {
            var doc = new MockDocumentWithStringCollection<T> { StringCollection = new T() };
            var expected = new List<string>();
            foreach (var value in values)
            {
                doc.StringCollection.Add(value);
                expected.Add(value);
            }
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(expected, attributes["StringCollection"].SS);
        }

        private class MockDocumentWithStringCollection<T>
        {
            public T StringCollection { get; set; }
        }
    }
}
