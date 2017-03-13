using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class UInt64Mapper
    {
        public static AttributeValue ToAttributeValue(ulong value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out ulong result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return ulong.TryParse(stringValue, out result);
            }
            result = default(ulong);
            return false;
        }
    }
}
