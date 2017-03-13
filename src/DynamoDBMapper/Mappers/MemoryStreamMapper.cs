using System;
using System.Collections.Generic;
using System.IO;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class MemoryStreamMapper
    {
        public static AttributeValue ToAttributeValue(MemoryStream value) => new AttributeValue { B = value };
        public static bool TryParseAttributeValue(AttributeValue value, out MemoryStream result)
        {
            if (value.B != null)
            {
                result = value.B;
                return true;
            }
            result = null;
            return false;
        }
    }
}
