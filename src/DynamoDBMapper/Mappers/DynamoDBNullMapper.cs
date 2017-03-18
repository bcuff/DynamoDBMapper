using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynamoDBMapper.Mappers
{
    internal static class DynamoDBNullMapper
    {
        public static AttributeValue ToAttributeValue(DynamoDBNull value)
        {
            return new AttributeValue { NULL = true };
        }

        public static bool TryParseAttributeValue(AttributeValue value, out DynamoDBNull result)
        {
            if (value.NULL)
            {
                result = DynamoDBNull.Null;
                return true;
            }
            result = null;
            return false;
        }
    }
}
