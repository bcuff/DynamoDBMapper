using System;
using System.Linq.Expressions;
using System.Reflection;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.TypeMappers
{
    internal class NullableTypeMapper : ITypeMapper
    {
        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
        {
            if (spec.IsNullableValueType)
            {
                var innerType = Nullable.GetUnderlyingType(spec.Type);
                var innerSpec = new TypeSpec(innerType, spec.ConverterType);
                var innerMapping = context.GetMapping(innerSpec);
                return new NullableTypeMapping(spec, innerSpec, innerMapping);
            }
            return null;
        }

        private class NullableTypeMapping : ITypeMapping
        {
            readonly TypeSpec _spec;
            readonly TypeSpec _innerSpec;
            readonly ITypeMapping _innerMapping;

            public NullableTypeMapping(TypeSpec spec, TypeSpec innerSpec, ITypeMapping innerMapping)
            {
                _spec = spec;
                _innerSpec = innerSpec;
                _innerMapping = innerMapping;
            }

            public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            {
                var result = Expression.Variable(_spec.Type);
                return Expression.Block(
                    new[] { result },
                    Expression.IfThenElse(
                        Expression.MakeMemberAccess(attributeValue, typeof(AttributeValue).GetProperty("NULL")),
                        Expression.Assign(result, Expression.Default(_spec.Type)),
                        Expression.Assign(
                            result,
                            Expression.Convert(
                                _innerMapping.GetFromAttributeValueExpression(context, attributeValue),
                                _spec.Type
                            )
                        )
                    ),
                    result
                );
            }

            public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
            {
                var result = Expression.Variable(typeof(AttributeValue));
                return Expression.Block(
                    new[] { result },
                    Expression.IfThenElse(
                        Expression.MakeMemberAccess(value, _spec.Type.GetProperty("HasValue")),
                        Expression.Assign(result, _innerMapping.GetToAttributeValueExpression(context, Expression.MakeMemberAccess(value, _spec.Type.GetProperty("Value")))),
                        Expression.Block(
                            Expression.Assign(result, Expression.New(typeof(AttributeValue))),
                            Expression.Assign(
                                Expression.MakeMemberAccess(result, typeof(AttributeValue).GetProperty("NULL")),
                                Expression.Constant(true)
                            )
                        )
                    ),
                    result
                );
            }
        }
    }
}
