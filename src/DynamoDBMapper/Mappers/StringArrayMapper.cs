using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class StringArrayMapper
    {
        public static AttributeValue ToAttributeValue(string[] value)
        {
            var ss = new List<string>(value.Length);
            ss.AddRange(value);
            return new AttributeValue { SS = ss };
        }

        public static bool TryParseAttributeValue(AttributeValue value, out string[] result)
        {
            if (value.SS != null)
            {
                result = value.SS.ToArray();
               return true;
            }
            result = null;
            return false;
        }
    }
}
