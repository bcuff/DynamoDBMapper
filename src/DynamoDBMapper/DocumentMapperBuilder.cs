using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amazon.DynamoDBv2.Model;
using DynamoDBMapper.TypeMappers;

namespace DynamoDBMapper
{
    public class DocumentMapperBuilder
    {
        private readonly List<ITypeMapper> _mappers = new List<ITypeMapper>();

        public DocumentMapper Create() => new DocumentMapper(_mappers);

        public DocumentMapperBuilder WithDefaults()
        {
            WithPropertyConverters();
            WithNullables();
            WithEnumerations();
            var q = from type in GetType().GetTypeInfo().Assembly.GetTypes()
                    where type.Namespace == "DynamoDBMapper.Mappers" && !type.IsNested
                    select type;
            foreach (var type in q)
            {
                WithMapperType(type);
            }
            WithNumericalArrays();
            return this;
        }

        public DocumentMapperBuilder WithNumericalArrays()
        {
            _mappers.Add(new NumericalArrayTypeMapper());
            return this;
        }

        public DocumentMapperBuilder WithPropertyConverters()
        {
            _mappers.Add(new PropertyConverterTypeMapper());
            return this;
        }

        public DocumentMapperBuilder WithNullables()
        {
            _mappers.Add(new NullableTypeMapper());
            return this;
        }

        public DocumentMapperBuilder WithEnumerations()
        {
            _mappers.Add(new EnumTypeMapper());
            return this;
        }

        public DocumentMapperBuilder WithMapperMethods(Type type, MethodInfo toAttributeValueMethod, MethodInfo parseAttributeValueMethod)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (toAttributeValueMethod == null) throw new ArgumentNullException(nameof(toAttributeValueMethod));
            if (parseAttributeValueMethod == null) throw new ArgumentNullException(nameof(parseAttributeValueMethod));
            if (!IsToAttributeValueMethod(type, toAttributeValueMethod)) new ArgumentNullException(nameof(toAttributeValueMethod), "Invalid signature.");
            if (!IsParseAttributeValueMethod(type, parseAttributeValueMethod)) new ArgumentNullException(nameof(parseAttributeValueMethod), "Invalid signature.");
            _mappers.Add(new PrimitiveTypeMapper(type, toAttributeValueMethod, parseAttributeValueMethod));
            return this;
        }

        public DocumentMapperBuilder WithMapperType(Type type)
        {
            var info = type.GetTypeInfo();
            var to = info.GetDeclaredMethod("ToAttributeValue");
            var parse = info.GetDeclaredMethod("TryParseAttributeValue");
            var toP = to.GetParameters();
            if (toP.Length != 1) throw new ArgumentException("Invalid mapper type");
            return WithMapperMethods(toP[0].ParameterType, to, parse);
        }

        private static bool IsToAttributeValueMethod(Type type, MethodInfo method)
        {
            var parameters = method.GetParameters();
            return method.ReturnType == typeof(AttributeValue)
                && parameters.Length == 1
                && parameters[0].ParameterType == type
                && parameters[0].IsIn && !parameters[0].IsOut;
        }

        private static bool IsParseAttributeValueMethod(Type type, MethodInfo method)
        {
            var parameters = method.GetParameters();
            return method.ReturnType == typeof(bool)
                && parameters.Length == 2
                && parameters[0].ParameterType == typeof(AttributeValue)
                && parameters[0].IsIn && !parameters[0].IsOut
                && parameters[1].ParameterType == type
                && parameters[1].IsIn && parameters[0].IsOut;
        }
    }
}
