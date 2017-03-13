using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class ByteMapper
    {
        public static AttributeValue ToAttributeValue(byte value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out byte result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return byte.TryParse(stringValue, out result);
            }
            result = default(byte);
            return false;
        }
    }
}
