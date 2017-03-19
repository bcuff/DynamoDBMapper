using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamoDBMapper.TypeMappers
{
    internal class BinaryCollectionTypeMapper : ITypeMapper
    {
        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
        {
            if (spec.ConverterType != null) return null;
            if (typeof(ICollection<byte[]>).GetTypeInfo().IsAssignableFrom(spec.TypeInfo)
                && !spec.TypeInfo.IsArray)
            {
                var ctor = spec.Type.GetConstructor(Type.EmptyTypes);
                if (ctor != null && ctor.IsPublic)
                {
                    return new BinaryCollectionTypeMapping(spec, typeof(GenericMappers.ByteArrayCollectionMapper));
                }
            }
            if (typeof(ICollection<MemoryStream>).GetTypeInfo().IsAssignableFrom(spec.TypeInfo)
                && !spec.TypeInfo.IsArray)
            {
                var ctor = spec.Type.GetConstructor(Type.EmptyTypes);
                if (ctor != null && ctor.IsPublic)
                {
                    return new BinaryCollectionTypeMapping(spec, typeof(GenericMappers.MemoryStreamCollectionMapper));
                }
            }
            return null;
        }

        private class BinaryCollectionTypeMapping : ITypeMapping
        {
            readonly TypeSpec _spec;
            readonly Type _mapperType;

            public BinaryCollectionTypeMapping(TypeSpec spec, Type mapperType)
            {
                _spec = spec;
                _mapperType = mapperType;
            }

            public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            {
                var method = _mapperType.GetMethod("TryParseAttributeValue").MakeGenericMethod(_spec.Type);
                return context.TryParseAttributeValue(method, attributeValue);
            }

            public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
            {
                var method = _mapperType.GetMethod("ToAttributeValue");
                return Expression.Call(method, value);
            }
        }
    }
}
