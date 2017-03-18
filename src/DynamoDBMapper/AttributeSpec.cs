using System;
using System.Reflection;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBMapper
{
    internal class AttributeSpec : TypeSpec
    {
        public AttributeSpec(PropertyInfo prop, DynamoDBRenamableAttribute attribute)
            : base(prop.PropertyType, (attribute as DynamoDBPropertyAttribute)?.Converter)
        {
            Property = prop;
            Attribute = attribute;
            Name = attribute?.AttributeName ?? prop.Name;
        }

        public PropertyInfo Property { get; }
        public DynamoDBRenamableAttribute Attribute { get; }
        public string Name { get; }
    }
}
