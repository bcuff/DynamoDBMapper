using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DynamoDBMapper
{
    internal interface ITypeMapper
    {
        ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context);
    }
}
