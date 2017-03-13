using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class UInt32Mapper
    {
        public static AttributeValue ToAttributeValue(uint value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out uint result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return uint.TryParse(stringValue, out result);
            }
            result = default(uint);
            return false;
        }
    }
}
