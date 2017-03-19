using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.TypeMappers
{
    internal class NumericalArrayTypeMapper : ITypeMapper
    {
        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
        {
            if (spec.ConverterType == null && spec.Type.IsArray)
            {
                var elementType = spec.Type.GetElementType();
                if (TypeHelper.IsNumericPrimitive(elementType))
                {
                    var tryParseMethod = TypeHelper.GetTryParseMethod(elementType);
                    if (tryParseMethod != null)
                    {
                        return new NumericalArrayTypeMapping(spec, elementType, tryParseMethod);
                    }
                }
            }
            return null;
        }

        private class NumericalArrayTypeMapping : ITypeMapping
        {
            readonly TypeSpec _spec;
            readonly Type _elementType;
            readonly MethodInfo _tryParseMethod;

            public NumericalArrayTypeMapping(TypeSpec spec, Type elementType, MethodInfo tryParseMethod)
            {
                _spec = spec;
                _elementType = elementType;
                _tryParseMethod = tryParseMethod;
            }

            public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            {
                var result = Expression.Variable(_spec.Type);
                var i = Expression.Variable(typeof(int));
                var temp = Expression.Variable(_elementType);
                var end = Expression.Label();
                var ns = Expression.MakeMemberAccess(attributeValue, typeof(AttributeValue).GetProperty("NS"));
                var count = Expression.MakeMemberAccess(ns, typeof(List<string>).GetProperty("Count"));
                var listIndexProprety = typeof(List<string>).GetProperty("Item", typeof(string), new[] { typeof(int) });
                return Expression.Block(
                    new[] { result, i, temp },
                    Expression.Assign(
                        result,
                        Expression.NewArrayBounds(
                            _elementType,
                            count
                        )
                    ),
                    Expression.Assign(i, Expression.Constant(0)),
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.LessThan(i, count),
                            Expression.Block(
                                Expression.Assign(
                                    Expression.ArrayAccess(result, i),
                                    Expression.Block(
                                        Expression.IfThen(
                                            Expression.Not(Expression.Call(
                                                _tryParseMethod,
                                                Expression.MakeIndex(ns, listIndexProprety, new[] { i }),
                                                temp
                                            )),
                                            context.GetThrowExpression()
                                        ),
                                        temp
                                    )
                                ),
                                Expression.PostIncrementAssign(i)
                            ),
                            Expression.Break(end)
                        ),
                        end
                    ),
                    result
                );
            }

            public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
            {
                var method = typeof(GenericMappers.NumericalEnumerableMapper).GetMethod("ToAttributeValue").MakeGenericMethod(_elementType);
                return Expression.Call(method, value);
            }
        }
    }
}
