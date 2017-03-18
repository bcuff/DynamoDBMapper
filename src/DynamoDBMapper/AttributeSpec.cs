using System;
using System.Reflection;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBMapper
{
    internal class AttributeSpec
    {
        public AttributeSpec(PropertyInfo prop, DynamoDBRenamableAttribute attribute)
        {
            var info = prop.PropertyType.GetTypeInfo();
            TargetType = prop.PropertyType;
            TargetTypeInfo = info;
            Property = prop;
            Attribute = attribute;
            Name = attribute?.AttributeName ?? prop.Name;
            ConverterType = (attribute as DynamoDBPropertyAttribute)?.Converter;
        }

        public Type TargetType { get; }
        public TypeInfo TargetTypeInfo { get; }
        public PropertyInfo Property { get; }
        public DynamoDBRenamableAttribute Attribute { get; }
        public string Name { get; }
        public Type ConverterType { get; }
    }
}
