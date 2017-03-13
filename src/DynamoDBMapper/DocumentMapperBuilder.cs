using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper
{
    public class DocumentMapperBuilder
    {
        private readonly Dictionary<Type, AttributeMapping> _mappers = new Dictionary<Type, AttributeMapping>();

        public DocumentMapper Create() => new DocumentMapper(_mappers);

        public DocumentMapperBuilder WithDefaults()
        {
            var q = from type in GetType().GetTypeInfo().Assembly.GetTypes()
                    where type.Namespace == "DynamoDBMapper.Mappers"
                    select type;
            foreach (var type in q)
            {
                WithMapperType(type);
            }
            return this;
        }

        public DocumentMapperBuilder WithMapperMethods(Type type, MethodInfo toAttributeValueMethod, MethodInfo parseAttributeValueMethod)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (toAttributeValueMethod == null) throw new ArgumentNullException(nameof(toAttributeValueMethod));
            if (parseAttributeValueMethod == null) throw new ArgumentNullException(nameof(parseAttributeValueMethod));
            if (!IsToAttributeValueMethod(type, toAttributeValueMethod)) new ArgumentNullException(nameof(toAttributeValueMethod), "Invalid signature.");
            if (!IsParseAttributeValueMethod(type, parseAttributeValueMethod)) new ArgumentNullException(nameof(parseAttributeValueMethod), "Invalid signature.");
            _mappers[type] = new AttributeMapping
            {
                To = toAttributeValueMethod,
                From = parseAttributeValueMethod
            };
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
