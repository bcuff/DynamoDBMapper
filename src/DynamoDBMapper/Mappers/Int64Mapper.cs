using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class Int64Mapper
    {
        public static AttributeValue ToAttributeValue(long value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out long result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return long.TryParse(stringValue, out result);
            }
            result = default(long);
            return false;
        }
    }
}
