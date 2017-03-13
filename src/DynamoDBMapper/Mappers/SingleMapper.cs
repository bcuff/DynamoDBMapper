using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class SingleMapper
    {
        public static AttributeValue ToAttributeValue(float value) => new AttributeValue { N = value.ToString("r", CultureInfo.InvariantCulture) };
        public static bool TryParseAttributeValue(AttributeValue value, out float result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            }
            result = default(float);
            return false;
        }
    }
}
