using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class Int16Mapper
    {
        public static AttributeValue ToAttributeValue(short value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out short result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return short.TryParse(stringValue, out result);
            }
            result = default(short);
            return false;
        }
    }
}
