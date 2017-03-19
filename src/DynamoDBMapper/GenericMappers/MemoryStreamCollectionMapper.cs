using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.GenericMappers
{
    internal static class MemoryStreamCollectionMapper
    {
        public static AttributeValue ToAttributeValue(ICollection<MemoryStream> value)
        {
            var ss = new List<MemoryStream>(value.Count);
            ss.AddRange(value);
            return new AttributeValue { BS = ss };
        }

        public static bool TryParseAttributeValue<T>(AttributeValue value, out T result)
            where T : ICollection<MemoryStream>, new()
        {
            if (value.SS != null)
            {
                result = new T();
                foreach (var v in value.BS)
                {
                    result.Add(v);
                }
                return true;
            }
            result = default(T);
            return false;
        }
    }
}
