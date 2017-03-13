using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class UInt16Mapper
    {
        public static AttributeValue ToAttributeValue(ushort value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out ushort result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return ushort.TryParse(stringValue, out result);
            }
            result = default(ushort);
            return false;
        }
    }
}
