using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;


namespace DynamoDBMapper.Mappers
{
    internal static class DocumentMapper
    {
        public static AttributeValue ToAttributeValue(Document value)
        {
            return new AttributeValue
            {
                M = value.ToDictionary(v => v.Key, v => DynamoDBEntryMapper.ToAttributeValue(v.Value))
            };
        }

        public static bool TryParseAttributeValue(AttributeValue value, out Document result)
        {
            if (value.M != null)
            {
                result = new Document();
                foreach (var kvp in value.M)
                {
                    DynamoDBEntry subEntry;
                    if (!DynamoDBEntryMapper.TryParseAttributeValue(kvp.Value, out subEntry))
                    {
                        throw new InvalidOperationException("Invalid document field.");
                    }
                    result[kvp.Key] = subEntry;
                }
                return true;
            }
            result = null;
            return false;
        }
    }
}
