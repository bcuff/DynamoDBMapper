using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper
{
    internal class PropertyConverterMapper : IPropertyMapper
    {
        public bool CanMap(AttributeSpec spec)
        {
            var converterType = (spec.Attribute as DynamoDBPropertyAttribute)?.Converter;
            if (converterType != null)
            {
                if (!typeof(IPropertyConverter).GetTypeInfo().IsAssignableFrom(converterType.GetTypeInfo()))
                {
                    throw new DynamoDBMapperException(spec.Property.Name, $"Converter of type {converterType.Name} does not implement {nameof(IPropertyConverter)}");
                }
                var ctor = converterType.GetConstructor(Type.EmptyTypes);
                if (ctor == null || !ctor.IsPublic)
                {
                    throw new DynamoDBMapperException(spec.Property.Name, $"Converter of type {converterType.Name} does not have a public default constructor.");
                }
                return true;
            }
            return false;
        }

        public Expression GetToAttributeValueExpression(AttributeSpec spec, Expression propertyValue)
        {
            var converterType = ((DynamoDBPropertyAttribute)spec.Attribute).Converter;
            var method = typeof(ConverterMapper).GetMethod("ToAttributeValue", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(converterType, spec.Property.PropertyType);
            return Expression.Call(method, propertyValue);
        }

        public Expression GetToDocumentPropertyExpression(AttributeSpec spec, Expression attributeValue)
        {
            var converterType = ((DynamoDBPropertyAttribute)spec.Attribute).Converter;
            var method = typeof(ConverterMapper).GetMethod("TryParseAttributeValue", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(converterType, spec.Property.PropertyType);
            var temp = Expression.Variable(spec.TargetType);
            return Expression.Block(
                new[] { temp },
                Expression.IfThen(
                    Expression.Not(Expression.Call(method, attributeValue, temp)),
                    Expression.Throw(
                        Expression.New(DynamoDBMapperException.Constructor, Expression.Constant(spec.Property.Name))
                    )
                ),
                temp
            );
        }
    }
}
