using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DynamoDBMapper.TypeMappers
{
    internal class PrimitivePropertyMapper : ITypeMapper, ITypeMapping
    {
        Type _type;
        MethodInfo _toAttributeValueMethod;
        MethodInfo _tryParseAttributeValueMethod;

        public PrimitivePropertyMapper(Type type, MethodInfo toAttributeValueMethod, MethodInfo tryParseAttributeValueMethod)
        {
            _type = type;
            _toAttributeValueMethod = toAttributeValueMethod;
            _tryParseAttributeValueMethod = tryParseAttributeValueMethod;
        }

        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
            => spec.Type == _type && spec.ConverterType == null ? this : null;

        public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
        {
            var temp = Expression.Variable(_type);
            return Expression.Block(
                new[] { temp },
                Expression.IfThen(
                    Expression.Not(Expression.Call(_tryParseAttributeValueMethod, attributeValue, temp)),
                    context.GetThrowExpression()
                ),
                temp
            );
        }

        public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
        {
            return Expression.Call(_toAttributeValueMethod, value);
        }
    }
}
