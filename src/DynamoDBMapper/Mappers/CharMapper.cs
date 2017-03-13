using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class CharMapper
    {
        public static AttributeValue ToAttributeValue(char value) => new AttributeValue { S = value.ToString() };
        public static bool TryParseAttributeValue(AttributeValue value, out char result)
        {
            var stringValue = value.S ?? value.N;
            if (stringValue != null)
            {
                return char.TryParse(stringValue, out result);
            }
            result = default(char);
            return false;
        }
    }
}
