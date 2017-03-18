using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynamoDBMapper.Mappers
{
    internal static class DynamoDBEntryMapper
    {
        public static AttributeValue ToAttributeValue(DynamoDBEntry entry)
        {
            var primitive = entry as Primitive;
            if (primitive != null)
            {
                return PrimitiveMapper.ToAttributeValue(primitive);
            }
            var pl = entry as PrimitiveList;
            if (pl != null)
            {
                return PrimitiveListMapper.ToAttributeValue(pl);
            }
            var @bool = entry as DynamoDBBool;
            if (@bool != null)
            {
                return DynamoDBBoolMapper.ToAttributeValue(@bool);
            }
            var @null = entry as DynamoDBNull;
            if (@null != null)
            {
                return DynamoDBNullMapper.ToAttributeValue(@null);
            }
            var l = entry as DynamoDBList;
            if (l != null)
            {
                return DynamoDBListMapper.ToAttributeValue(l);
            }
            var d = entry as Document;
            if (d != null)
            {
                return DocumentMapper.ToAttributeValue(d);
            }
            throw new ArgumentException("Invalid entry", nameof(entry));
        }

        public static bool TryParseAttributeValue(AttributeValue value, out DynamoDBEntry entry)
        {
            entry = null;
            Primitive primitive;
            if (PrimitiveMapper.TryParseAttributeValue(value, out primitive))
            {
                entry = primitive;
                return true;
            }
            PrimitiveList pl;
            if (PrimitiveListMapper.TryParseAttributeValue(value, out pl))
            {
                entry = pl;
                return true;
            }
            DynamoDBBool @bool;
            if (DynamoDBBoolMapper.TryParseAttributeValue(value, out @bool))
            {
                entry = @bool;
                return true;
            }
            DynamoDBNull @null;
            if (DynamoDBNullMapper.TryParseAttributeValue(value, out @null))
            {
                entry = @null;
                return true;
            }
            DynamoDBList l;
            if (DynamoDBListMapper.TryParseAttributeValue(value, out l))
            {
                entry = l;
                return true;
            }
            Document d;
            if (DocumentMapper.TryParseAttributeValue(value, out d))
            {
                entry = d;
                return true;
            }
            return false;
        }
    }
}
