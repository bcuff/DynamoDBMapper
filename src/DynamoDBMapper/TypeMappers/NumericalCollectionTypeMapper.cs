using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.TypeMappers
{
    internal class NumericalCollectionTypeMapper : ITypeMapper
    {
        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
        {
            if (spec.ConverterType != null) return null;
            if (spec.Type.IsArray) return null;
            var q = from inter in spec.Type.GetInterfaces()
                    let interInfo = inter.GetTypeInfo()
                    where interInfo.IsGenericType && interInfo.GetGenericTypeDefinition() == typeof(ICollection<>)
                    let elementType = inter.GetGenericArguments()[0]
                    where TypeHelper.IsNumericPrimitive(elementType)
                    let tryParse = TypeHelper.GetTryParseMethod(elementType)
                    where tryParse != null
                    select new { inter, elementType, tryParse };
            var target = q.FirstOrDefault();
            if (target == null) return null;
            var ctor = spec.Type.GetConstructor(Type.EmptyTypes);
            if (ctor != null && ctor.IsPublic)
            {
                return new NumericalCollectionTypeMapping(spec, target.inter, target.elementType, target.tryParse);
            }

            return null;
        }

        private class NumericalCollectionTypeMapping : ITypeMapping
        {
            readonly TypeSpec _spec;
            readonly Type _collectionType;
            readonly Type _elementType;
            readonly MethodInfo _tryParse;

            public NumericalCollectionTypeMapping(TypeSpec spec, Type collectionType, Type elementType, MethodInfo tryParse)
            {
                _spec = spec;
                _collectionType = collectionType;
                _elementType = elementType;
                _tryParse = tryParse;
            }

            public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
            {
                var method = typeof(GenericMappers.NumericalEnumerableMapper).GetMethod("ToAttributeValue").MakeGenericMethod(_elementType);
                return Expression.Call(method, value);
            }

            public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            {
                var result = Expression.Variable(_spec.Type);
                var ns = Expression.Variable(typeof(List<string>));
                var i = Expression.Variable(typeof(int));
                var temp = Expression.Variable(_elementType);
                var count = Expression.MakeMemberAccess(ns, typeof(List<string>).GetProperty(nameof(List<string>.Count)));
                var end = Expression.Label();
                var listIndexProprety = typeof(List<string>).GetProperty("Item", typeof(string), new[] { typeof(int) });
                return Expression.Block(
                    new[] { result, ns },
                    Expression.Assign(ns, Expression.MakeMemberAccess(attributeValue, typeof(AttributeValue).GetProperty("NS"))),
                    Expression.IfThenElse(
                        Expression.NotEqual(ns, Expression.Constant(null)),
                        Expression.Block(
                            new[] { i },
                            Expression.Assign(i, Expression.Constant(0)),
                            Expression.Assign(result, Expression.New(_spec.Type)),
                            Expression.Loop(
                                Expression.IfThenElse(
                                    Expression.LessThan(i, count),
                                    Expression.Block(
                                        Expression.Call(
                                            result,
                                            _collectionType.GetMethod("Add"),
                                            Expression.Block(
                                                new[] { temp },
                                                Expression.IfThen(
                                                    Expression.Not(Expression.Call(
                                                        _tryParse,
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
                            )
                        ),
                        Expression.Assign(result, Expression.Default(_spec.Type))
                    ),
                    result
                );
            }
        }
    }
}
