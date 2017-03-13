using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class BooleanMapper
    {
        public static AttributeValue ToAttributeValue(bool value) => new AttributeValue { BOOL = value };
        public static bool TryParseAttributeValue(AttributeValue value, out bool result)
        {
            if (value.IsBOOLSet)
            {
                result = value.BOOL;
                return true;
            }
            if (!string.IsNullOrEmpty(value.N))
            {
                result = value.N != "0";
                return true;
            }
            result = default(bool);
            return false;
        }
    }
}
