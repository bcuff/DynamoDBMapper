using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Tests
{
    public class NumericalArrayTests
    {
        public static TheoryData<double[], List<string>> NumericalArrayData { get; } = new TheoryData<double[], List<string>>
        {
            { new double[] { 0, -1, Math.PI }, new List<string> { "0", "-1", $"{Math.PI}" } }
        };

        [Theory]
        [MemberData(nameof(NumericalArrayData))]
        public void DoubleArray_converts_to_number_set(double[] array, List<string> expectedNSValue)
        {
            var doc = new MockDocument { DoubleArray = array };
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(expectedNSValue, attributes[nameof(doc.DoubleArray)].NS);
        }

        [Theory]
        [MemberData(nameof(NumericalArrayData))]
        public void DoubleArray_converts_from_number_set(double[] expectedArray, List<string> nsValue)
        {
            var attributes = new Dictionary<string, AttributeValue> { { nameof(MockDocument.DoubleArray), new AttributeValue { NS = nsValue } } };
            var doc = DocumentMapper.Default.ToDocument<MockDocument>(attributes);
            Assert.Equal(expectedArray.Length, doc.DoubleArray.Length);
            for (int i = 0; i < expectedArray.Length; ++i)
            {
                Assert.True(Math.Abs(doc.DoubleArray[i] - expectedArray[i]) < .00001d);
            }
        }
    }
}
