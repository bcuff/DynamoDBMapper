using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.GenericMappers
{
    internal static class EnumMapper
    {
        private static class EnumCache<T>
        {
            public static Dictionary<string, T> ValuesByName;

            static EnumCache()
            {
                ValuesByName = Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .ToDictionary(v => v.ToString(), v => v);
            }
        }

        public static bool TryParseAttributeValue<T>(AttributeValue value, out T result)
        {
            if (value.S != null)
            {
                return EnumCache<T>.ValuesByName.TryGetValue(value.S, out result);
            }
            result = default(T);
            return false;
        }
    }
}
