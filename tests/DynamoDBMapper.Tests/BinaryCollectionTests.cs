using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Xunit;

namespace DynamoDBMapper.Tests
{
    public static class BinaryCollectionTests
    {
        public static TheoryData<byte[][]> Data = new TheoryData<byte[][]>
        {
            { new byte[0][] },
            { new byte[][] { Guid.NewGuid().ToByteArray(), Guid.NewGuid().ToByteArray() } },
        };

        [Theory]
        [MemberData(nameof(Data))]
        public static void ArrayOfByteArray_should_write_to_binary_set(byte[][] data)
        {
            var doc = new MockGenericDocument<byte[][]>() { Value = data };
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(data.Length, attributes["Value"].BS.Count);
            for (int i = 0; i < data.Length; ++i)
            {
                Assert.Equal(data[i], attributes["Value"].BS[i].ToArray());
            }
        }
    }
}
