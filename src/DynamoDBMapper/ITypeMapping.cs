using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DynamoDBMapper
{
    interface ITypeMapping
    {
        Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value);
        Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue);
    }
}
