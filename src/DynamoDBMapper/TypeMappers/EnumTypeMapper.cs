using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper.TypeMappers
{
    internal class EnumTypeMapper : ITypeMapper
    {
        public ITypeMapping GetTypeMapping(TypeSpec spec, IMapperGeneratorContext context)
        {
            if (!spec.TypeInfo.IsEnum && spec.ConverterType == null) return null;
            var innerSpec = new TypeSpec(Enum.GetUnderlyingType(spec.Type));
            var innerMapping = context.GetMapping(innerSpec);
            return new EnumTypeMapping(spec, innerSpec, innerMapping);
        }

        private class EnumTypeMapping : ITypeMapping
        {
            TypeSpec _spec;
            TypeSpec _underlyingSpec;
            ITypeMapping _underlyingMapping;

            public EnumTypeMapping(TypeSpec spec, TypeSpec underlyingSpec, ITypeMapping underlyingMapping)
            {
                _spec = spec;
                _underlyingSpec = underlyingSpec;
                _underlyingMapping = underlyingMapping;
            }

            public Expression GetFromAttributeValueExpression(IMapperGeneratorContext context, Expression attributeValue)
            {
                var temp = Expression.Variable(_spec.Type);
                return Expression.Block(
                        new[] { temp },
                        Expression.IfThen(
                            Expression.Not(
                                Expression.Call(
                                    typeof(GenericMappers.EnumMapper).GetMethod("TryParseAttributeValue").MakeGenericMethod(_spec.Type),
                                    attributeValue,
                                    temp
                                )
                            ),
                            Expression.Assign(
                                temp,
                                Expression.Convert(
                                    _underlyingMapping.GetFromAttributeValueExpression(context, attributeValue),
                                    _spec.Type
                                )
                            )
                        ),
                        temp
                    );
            }

            public Expression GetToAttributeValueExpression(IMapperGeneratorContext context, Expression value)
            {
                return _underlyingMapping.GetToAttributeValueExpression(
                    context,
                    Expression.Convert(value, _underlyingSpec.Type));
            }
        }
    }
}
