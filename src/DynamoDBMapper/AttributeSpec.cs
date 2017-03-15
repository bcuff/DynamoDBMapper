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
            if (info.IsNullable())
            {
                TargetType = Nullable.GetUnderlyingType(prop.PropertyType);
                TargetTypeInfo = TargetType.GetTypeInfo();
                IsNullable = true;
            }
            else
            {
                TargetType = prop.PropertyType;
                TargetTypeInfo = info;
                IsNullable = !TargetTypeInfo.IsValueType;
            }
            Property = prop;
            Attribute = attribute;
            Name = attribute?.AttributeName ?? prop.Name;
        }

        public Type TargetType { get; }
        public TypeInfo TargetTypeInfo { get; }
        public PropertyInfo Property { get; }
        public DynamoDBRenamableAttribute Attribute { get; }
        public string Name { get; }
        public bool IsNullable { get; }
    }
}
