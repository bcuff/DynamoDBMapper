using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.GenericMappers
{
    internal static class StringEnumerableMapper
    {
        public static AttributeValue ToAttributeValue<T>(T value)
            where T : IEnumerable<string>
        {
            return new AttributeValue
            {
                SS = value.ToList()
            };
        }
    }
}
