using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.GenericMappers
{
    internal static class NumericalEnumerableMapper
    {
        public static AttributeValue ToAttributeValue<T>(IEnumerable<T> value)
        {
            return new AttributeValue
            {
                NS = value.Select(v => v.ToString()).ToList()
            };
        }
    }
}
