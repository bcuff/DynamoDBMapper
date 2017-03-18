using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;


namespace DynamoDBMapper.Mappers
{
    internal static class DynamoDBListMapper
    {
        public static AttributeValue ToAttributeValue(DynamoDBList value)
        {
            return new AttributeValue
            {
                L = value.Entries.Select(DynamoDBEntryMapper.ToAttributeValue).ToList()
            };
        }

        public static bool TryParseAttributeValue(AttributeValue value, out DynamoDBList result)
        {
            if (value.L != null)
            {
                result = new DynamoDBList();
                foreach (var item in value.L)
                {
                    DynamoDBEntry subEntry;
                    if (!DynamoDBEntryMapper.TryParseAttributeValue(item, out subEntry))
                    {
                        throw new InvalidOperationException("Invalid DynamoDBList");
                    }
                    result.Add(subEntry);
                }
                return true;
            }
            result = null;
            return false;
        }
    }
}
