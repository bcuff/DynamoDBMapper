using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.GenericMappers
{
    internal static class StringCollectionMapper
    {
        public static AttributeValue ToAttributeValue(ICollection<string> value)
        {
            var ss = new List<string>(value.Count);
            ss.AddRange(value);
            return new AttributeValue { SS = ss };
        }

        public static bool FromAttributeValue<T>(AttributeValue value, out T result)
            where T : ICollection<string>, new()
        {
            if (value.SS != null)
            {
                result = new T();
                foreach (var v in value.SS)
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
