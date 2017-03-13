using System;
using System.Collections.Generic;
using System.IO;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class ByteArrayMapper
    {
        public static AttributeValue ToAttributeValue(byte[] value) => new AttributeValue { B = new MemoryStream(value) };
        public static bool TryParseAttributeValue(AttributeValue value, out byte[] result)
        {
            if (value.B != null)
            {
                result = value.B.ToArray();
                return true;
            }
            result = null;
            return false;
        }
    }
}
