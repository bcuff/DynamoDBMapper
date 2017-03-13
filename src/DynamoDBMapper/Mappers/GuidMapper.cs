using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class GuidMapper
    {
        public static AttributeValue ToAttributeValue(Guid value) => new AttributeValue { S = value.ToString("D") };
        public static bool TryParseAttributeValue(AttributeValue value, out Guid result)
        {
            var stringValue = value.S ?? value.N;
            if (stringValue != null && Guid.TryParse(stringValue, out result))
            {
                return true;
            }
            result = default(Guid);
            return false;
        }
    }
}
