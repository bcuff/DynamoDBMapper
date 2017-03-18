using System;
using System.IO;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynamoDBMapper.Mappers
{
    internal static class PrimitiveMapper
    {
        public static AttributeValue ToAttributeValue(Primitive primitive)
        {
            if (primitive.Value == null) return null;
            string stringValue;
            switch (primitive.Type)
            {
                case DynamoDBEntryType.String:
                    stringValue = primitive.Value as string;
                    if (stringValue == null) return null;
                    return new AttributeValue { S = stringValue };
                case DynamoDBEntryType.Numeric:
                    stringValue = primitive.Value as string;
                    if (stringValue == null) return null;
                    return new AttributeValue { N = stringValue };
                case DynamoDBEntryType.Binary:
                    var bytes = primitive.Value as byte[];
                    if (bytes == null) return null;
                    return new AttributeValue { B = new MemoryStream(bytes) };
            }
            return null;
        }

        public static bool TryParseAttributeValue(AttributeValue value, out Primitive result)
        {
            if (value.S != null)
            {
                result = new Primitive(value.S);
                return true;
            }
            if (value.N != null)
            {
                result = new Primitive(value.N, true);
                return true;
            }
            if (value.B != null)
            {
                result = new Primitive(value.B);
                return true;
            }
            result = null;
            return false;
        }
    }
}
