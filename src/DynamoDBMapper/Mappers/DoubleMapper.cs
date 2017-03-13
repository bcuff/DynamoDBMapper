using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.Mappers
{
    internal static class DoubleMapper
    {
        public static AttributeValue ToAttributeValue(double value) => new AttributeValue { N = value.ToString("r", CultureInfo.InvariantCulture) };
        public static bool TryParseAttributeValue(AttributeValue value, out double result)
        {
            var stringValue = value.N ?? value.S;
            if (stringValue != null)
            {
                return double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            }
            result = default(double);
            return false;
        }
    }
}
