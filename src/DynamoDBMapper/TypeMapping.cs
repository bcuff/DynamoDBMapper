using System;
using System.Linq.Expressions;

namespace DynamoDBMapper
{
    internal class SimpleTypeMapping : ITypeMapping
    {
        readonly Func<IMapperGeneratorContext, Expression, Expression> _toAttributeValueExpression;
        readonly Func<IMapperGeneratorContext, Expression, Expression> _fromAttributeValueExpression;

        public SimpleTypeMapping(
            Func<IMapperGeneratorContext, Expression, Expression> toAttributeValueExpression,
            Func<IMapperGeneratorContext, Expression, Expression> fromAttributeValueExpression)
        {
            _toAttributeValueExpression = toAttributeValueExpression;
            _fromAttributeValueExpression = fromAttributeValueExpression;
        }

        public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
            => _toAttributeValueExpression(context, value);

        public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            => _fromAttributeValueExpression(context, attributeValue);
    }
}
