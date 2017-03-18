using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.TypeMappers
{
    internal class PropertyConverterMapper : ITypeMapper
    {
        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
        {
            if (spec.ConverterType == null) return null;
            var converterTypeInfo = spec.ConverterType.GetTypeInfo();
            if (!typeof(IPropertyConverter).GetTypeInfo().IsAssignableFrom(converterTypeInfo))
            {
                throw new ArgumentException($"Converter of type {spec.ConverterType.Name} does not implement {nameof(IPropertyConverter)}", nameof(spec));
            }
            var ctor = spec.ConverterType.GetConstructor(Type.EmptyTypes);
            if (ctor == null || !ctor.IsPublic)
            {
                throw new ArgumentException($"Converter of type {spec.ConverterType.Name} does not have a public default constructor.", nameof(spec));
            }
            return new PropertyConverterTypeMapping(spec);
        }

        private class PropertyConverterTypeMapping : ITypeMapping
        {
            readonly TypeSpec _spec;

            public PropertyConverterTypeMapping(TypeSpec spec)
            {
                _spec = spec;
            }

            public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            {
                var method = typeof(GenericMappers.ConverterMapper).GetMethod("TryParseAttributeValue", BindingFlags.Static | BindingFlags.Public)
                    .MakeGenericMethod(_spec.ConverterType, _spec.Type);
                var temp = Expression.Variable(_spec.Type);
                return Expression.Block(
                    new[] { temp },
                    Expression.IfThen(
                        Expression.Not(Expression.Call(method, attributeValue, temp)),
                        context.GetThrowExpression()
                    ),
                    temp
                );
            }

            public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
            {
                var method = typeof(GenericMappers.ConverterMapper).GetMethod("ToAttributeValue", BindingFlags.Static | BindingFlags.Public)
                    .MakeGenericMethod(_spec.ConverterType, _spec.Type);
                return Expression.Call(method, value);
            }
        }
    }
}
