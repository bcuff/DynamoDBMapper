using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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

        [Theory]
        [MemberData(nameof(Data))]
        public static void ArrayOfByteArray_should_read_from_binary_set(byte[][] data)
        {
            var attributes = new Dictionary<string, AttributeValue> { { "Value", new AttributeValue { BS = data.Select(v => new MemoryStream(v)).ToList() } } };
            var doc = DocumentMapper.Default.ToDocument<MockGenericDocument<byte[][]>>(attributes);
            Assert.Equal(data.Length, doc.Value.Length);
            for (int i = 0; i < data.Length; ++i)
            {
                Assert.Equal(data[i], doc.Value[i]);
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public static void CollectionsOfByteArray_should_write_to_binary_set(byte[][] data)
        {
            CollectionsOfByteArray_should_write_to_binary_set<List<byte[]>>(data);
            CollectionsOfByteArray_should_write_to_binary_set<HashSet<byte[]>>(data);
        }

        public static void CollectionsOfByteArray_should_write_to_binary_set<T>(byte[][] data)
            where T : ICollection<byte[]>, new()
        {
            var expected = new T();
            foreach (var v in data) expected.Add(v);
            var doc = new MockGenericDocument<T>() { Value = expected };
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(data.Length, attributes["Value"].BS.Count);
            for (int i = 0; i < data.Length; ++i)
            {
                Assert.Equal(data[i], attributes["Value"].BS[i].ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public static void CollectionsOfByteArray_should_read_from_binary_set(byte[][] data)
        {
            CollectionsOfByteArray_should_read_from_binary_set<List<byte[]>>(data);
            CollectionsOfByteArray_should_read_from_binary_set<HashSet<byte[]>>(data);
        }

        public static void CollectionsOfByteArray_should_read_from_binary_set<T>(byte[][] data)
            where T : ICollection<byte[]>, new()
        {
            var attributes = new Dictionary<string, AttributeValue> { { "Value", new AttributeValue { BS = data.Select(v => new MemoryStream(v)).ToList() } } };
            var doc = DocumentMapper.Default.ToDocument<MockGenericDocument<T>>(attributes);
            Assert.Equal(data.Length, doc.Value.Count);
            Assert.Equal(data, doc.Value, ByteArrayEqualityComparer.Instance);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public static void CollectionsOfMemoryStream_should_write_to_binary_set(byte[][] data)
        {
            CollectionsOfMemoryStream_should_write_to_binary_set<List<MemoryStream>>(data);
            CollectionsOfMemoryStream_should_write_to_binary_set<HashSet<MemoryStream>>(data);
        }

        public static void CollectionsOfMemoryStream_should_write_to_binary_set<T>(byte[][] data)
            where T : ICollection<MemoryStream>, new()
        {
            var expected = new T();
            foreach (var v in data) expected.Add(new MemoryStream(v));
            var doc = new MockGenericDocument<T>() { Value = expected };
            var attributes = DocumentMapper.Default.ToAttributes(doc);
            Assert.Equal(data.Length, attributes["Value"].BS.Count);
            for (int i = 0; i < data.Length; ++i)
            {
                Assert.Equal(data[i], attributes["Value"].BS[i].ToArray());
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public static void CollectionsOfMemoryStream_should_read_from_binary_set(byte[][] data)
        {
            CollectionsOfMemoryStream_should_read_from_binary_set<List<MemoryStream>>(data);
            CollectionsOfMemoryStream_should_read_from_binary_set<HashSet<MemoryStream>>(data);
        }

        public static void CollectionsOfMemoryStream_should_read_from_binary_set<T>(byte[][] data)
            where T : ICollection<MemoryStream>, new()
        {
            var attributes = new Dictionary<string, AttributeValue> { { "Value", new AttributeValue { BS = data.Select(v => new MemoryStream(v)).ToList() } } };
            var doc = DocumentMapper.Default.ToDocument<MockGenericDocument<T>>(attributes);
            Assert.Equal(data.Length, doc.Value.Count);
            Assert.Equal(data, doc.Value.Select(v => v.ToArray()), ByteArrayEqualityComparer.Instance);
        }
    }
}
