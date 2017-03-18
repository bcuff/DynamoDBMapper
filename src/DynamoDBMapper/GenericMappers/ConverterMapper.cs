using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DynamoDBMapper.Mappers;

namespace DynamoDBMapper.GenericMappers
{
    internal static class ConverterMapper
    {
        private static class ConverterCache<T>
            where T : IPropertyConverter, new()
        {
            public static T Converter { get; } = new T();
        }

        public static AttributeValue ToAttributeValue<TConverter, TValue>(TValue value)
            where TConverter : IPropertyConverter, new()
        {
            var converter = ConverterCache<TConverter>.Converter;
            var entry = converter.ToEntry(value);
            return DynamoDBEntryMapper.ToAttributeValue(entry);
        }

        public static bool TryParseAttributeValue<TConverter, TValue>(AttributeValue value, out TValue result)
            where TConverter : IPropertyConverter, new()
        {
            DynamoDBEntry entry;
            if (DynamoDBEntryMapper.TryParseAttributeValue(value, out entry))
            {
                var converter = ConverterCache<TConverter>.Converter;
                var objectResult = converter.FromEntry(entry);
                if (objectResult is TValue)
                {
                    result = (TValue)objectResult;
                    return true;
                }
            }
            result = default(TValue);
            return false;
        }
    }
}
