using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class SByteMapper
    {
        public static AttributeValue ToAttributeValue(sbyte value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out sbyte result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return sbyte.TryParse(stringValue, out result);
            }
            result = default(sbyte);
            return false;
        }
    }
}
