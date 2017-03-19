using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamoDBMapper.TypeMappers
{
    internal class StringCollectionTypeMapper : ITypeMapper
    {
        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
        {
            if (spec.ConverterType != null) return null;
            if (typeof(ICollection<string>).GetTypeInfo().IsAssignableFrom(spec.TypeInfo)
                && !spec.TypeInfo.IsArray)
            {
                var ctor = spec.Type.GetConstructor(Type.EmptyTypes);
                if (ctor != null && ctor.IsPublic)
                {
                    return new StringCollectionTypeMapping(spec);
                }
            }
            return null;
        }

        private class StringCollectionTypeMapping : ITypeMapping
        {
            readonly TypeSpec _spec;

            public StringCollectionTypeMapping(TypeSpec spec)
            {
                _spec = spec;
            }

            public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            {
                var method = typeof(GenericMappers.StringCollectionMapper)
                    .GetMethod(nameof(GenericMappers.StringCollectionMapper.FromAttributeValue))
                    .MakeGenericMethod(_spec.Type);
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
                var method = typeof(GenericMappers.StringCollectionMapper).GetMethod(nameof(GenericMappers.StringCollectionMapper.ToAttributeValue));
                return Expression.Call(method, value);
            }
        }
    }
}
