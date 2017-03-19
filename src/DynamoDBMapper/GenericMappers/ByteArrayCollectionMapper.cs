using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.GenericMappers
{
    internal static class ByteArrayCollectionMapper
    {
        public static AttributeValue ToAttributeValue(ICollection<byte[]> value)
        {
            var ss = new List<MemoryStream>(value.Count);
            ss.AddRange(value.Select(v => new MemoryStream(v)));
            return new AttributeValue { BS = ss };
        }

        public static bool TryParseAttributeValue<T>(AttributeValue value, out T result)
            where T : ICollection<byte[]>, new()
        {
            if (value.SS != null)
            {
                result = new T();
                foreach (var v in value.BS)
                {
                    result.Add(v.ToArray());
                }
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
