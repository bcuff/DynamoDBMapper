using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class DecimalMapper
    {
        public static AttributeValue ToAttributeValue(decimal value) => new AttributeValue { N = value.ToString("g", CultureInfo.InvariantCulture) };
        public static bool TryParseAttributeValue(AttributeValue value, out decimal result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return decimal.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            }
            result = default(decimal);
            return false;
        }
    }
}
