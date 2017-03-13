using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Amazon.Util;

namespace DynamoDBMapper.Mappers
{
    internal static class DateTimeMapper
    {
        public static AttributeValue ToAttributeValue(DateTime value) => new AttributeValue { S = value.ToUniversalTime().ToString(AWSSDKUtils.ISO8601DateFormat, CultureInfo.InvariantCulture) };
        public static bool TryParseAttributeValue(AttributeValue value, out DateTime result)
        {
            var stringValue = value.S ?? value.N;
            if (stringValue != null)
            {
                return DateTime.TryParseExact(stringValue, AWSSDKUtils.ISO8601DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result);
            }
            result = default(DateTime);
            return false;
        }
    }
}
