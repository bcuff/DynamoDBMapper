using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public static class NumericalCollectionTests
    {
        public static TheoryData<short[], List<string>> Values = new TheoryData<short[], List<string>>
        {
            { new short[] { -1, 0, 234 }, new List<string> { "-1", "0", "234" } }
        };

        [Theory]
        [MemberData(nameof(Values))]
        public static void NumericalCollections_map_to_number_sets(short[] values, List<string> expectedNSValues)
        {
            NumericalCollections_map_to_number_sets<HashSet<short>>(values, expectedNSValues);
            NumericalCollections_map_to_number_sets<List<short>>(values, expectedNSValues);
        }

        public static void NumericalCollections_map_to_number_sets<T>(short[] values, List<string> expectedNSValues)
            where T : ICollection<short>, new()
        {
            var doc = new MockGenericDocument<T>{ Value = new T() };
            foreach (var v in values) doc.Value.Add(v);
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(expectedNSValues, attributes["Value"].NS);
        }
    }
}
