using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;


namespace DynamoDBMapper.Mappers
{
    internal static class DynamoDBBoolMapper
    {
        public static AttributeValue ToAttributeValue(DynamoDBBool value)
        {
            return new AttributeValue { BOOL = value.Value };
        }

        public static bool TryParseAttributeValue(AttributeValue value, out DynamoDBBool result)
        {
            if (value.IsBOOLSet)
            {
                result = new DynamoDBBool(value.BOOL);
                return true;
            }
            result = null;
            return false;
        }
    }
}
