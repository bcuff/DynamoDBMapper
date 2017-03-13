using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class Int32Mapper
    {
        public static AttributeValue ToAttributeValue(int value) => new AttributeValue { N = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out int result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return int.TryParse(stringValue, out result);
            }
            result = default(int);
            return false;
        }
    }
}
