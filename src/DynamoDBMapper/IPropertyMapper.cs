using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamoDBMapper
{
    internal interface IPropertyMapper
    {
        bool CanMap(AttributeSpec spec);
        Expression GetToDocumentPropertyExpression(AttributeSpec spec, Expression attributeValue);
        Expression GetToAttributeValueExpression(AttributeSpec spec, Expression propertyValue);
    }
}
