using System;
using System.Linq.Expressions;

namespace DynamoDBMapper
{
    interface IMapperGeneratorContext
    {
        ITypeMapping GetMapping(TypeSpec typeSpec);
        Expression GetThrowExpression();
    }
}
