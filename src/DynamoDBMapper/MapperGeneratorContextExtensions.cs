using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper
{
    internal static class MapperGeneratorContextExtensions
    {
        public static Expression TryParseAttributeValue(this IMapperGeneratorContext context, MethodInfo tryParseMethod, Expression attributeValue)
        {
            var p = tryParseMethod.GetParameters();
            if (p.Length != 2
                || p[0].ParameterType != typeof(AttributeValue)
                || !p[1].IsOut
                || !p[1].ParameterType.IsByRef)
            {
                throw new ArgumentException("Invalid try parse signature", nameof(tryParseMethod));
            }
            var temp = Expression.Variable(p[1].ParameterType.GetElementType());
            return Expression.Block(
                new[] { temp },
                Expression.IfThen(
                    Expression.Not(Expression.Call(tryParseMethod, attributeValue, temp)),
                    context.GetThrowExpression()
                ),
                temp
            );
        }
    }
}
