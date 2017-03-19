using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class ByteArrayArrayMapper
    {
        public static AttributeValue ToAttributeValue(byte[][] value) => new AttributeValue { BS = value.Select(v => new MemoryStream(v)).ToList() };
        public static bool TryParseAttributeValue(AttributeValue value, out byte[][] result)
        {
            if (value.BS != null)
            {
                result = value.BS.Select(v => v.ToArray()).ToArray();
                return true;
            }
            result = null;
            return false;
        }
    }
}
