using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class StringMapper
    {
        public static AttributeValue ToAttributeValue(string value) => string.IsNullOrEmpty(value) ? null : new AttributeValue { S = value };
        public static bool TryParseAttributeValue(AttributeValue value, out string result)
        {
            if (value.S != null)
            {
                result = value.S;
                return true;
            }
            if (value.N != null)
            {
                result = value.N;
                return true;
            }
            result = null;
            return false;
        }
    }
}
