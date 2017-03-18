using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DynamoDBMapper
{
    internal class PrimitivePropertyMapper : IPropertyMapper
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

        public bool CanMap(AttributeSpec spec)
            => spec.TargetType == _type && spec.ConverterType == null;

        public Expression GetToAttributeValueExpression(AttributeSpec spec, Expression propertyValue)
        {
            return Expression.Call(_toAttributeValueMethod, propertyValue);
        }

        public Expression GetToDocumentPropertyExpression(AttributeSpec spec, Expression attributeValue)
        {
            var temp = Expression.Variable(spec.TargetType);
            return Expression.Block(
                new[] { temp },
                Expression.IfThen(
                    Expression.Not(Expression.Call(_tryParseAttributeValueMethod, attributeValue, temp)),
                    Expression.Throw(
                        Expression.New(DynamoDBMapperException.Constructor, Expression.Constant(spec.Property.Name))
                    )
                ),
                temp
            );
        }
    }
}
