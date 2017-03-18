using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;


namespace DynamoDBMapper.Mappers
{
    internal static class PrimitiveListMapper
    {
        public static AttributeValue ToAttributeValue(PrimitiveList value)
        {
            if (value.Type == DynamoDBEntryType.Numeric || value.Type == DynamoDBEntryType.String)
            {
                var values = new List<string>();
                foreach (var entry in value.Entries)
                {
                    values.Add((string)entry.Value);
                }
                return value.Type == DynamoDBEntryType.Numeric
                    ? new AttributeValue { NS = values }
                    : new AttributeValue { SS = values };
            }
            if (value.Type == DynamoDBEntryType.Binary)
            {
                return new AttributeValue { BS = value.Entries.Select(v => new MemoryStream(v.AsByteArray())).ToList() };
            }
            throw new InvalidOperationException("Unsupported type");
        }

        public static bool TryParseAttributeValue(AttributeValue value, out PrimitiveList result)
        {
            if (value.SS != null)
            {
                result = new PrimitiveList(DynamoDBEntryType.String);
                foreach (var stringValue in value.SS)
                {
                    result.Add(new Primitive(stringValue));
                }
                return true;
            }
            if (value.NS != null)
            {
                result = new PrimitiveList(DynamoDBEntryType.Numeric);
                foreach (var numericValue in value.NS)
                {
                    result.Add(new Primitive(numericValue, true));
                }
                return true;
            }
            if (value.BS != null)
            {
                result = new PrimitiveList(DynamoDBEntryType.Binary);
                foreach (var binaryValue in value.BS)
                {
                    result.Add(new Primitive(binaryValue));
                }
                return true;
            }
            result = null;
            return false;
        }
    }
}
