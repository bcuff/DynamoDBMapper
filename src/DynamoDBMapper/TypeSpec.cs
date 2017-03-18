using System;
using System.Reflection;

namespace DynamoDBMapper
{
    internal class TypeSpec
    {
        public TypeSpec(Type type)
            : this(type, null)
        {
        }

        public TypeSpec(Type type, Type converterType)
        {
            Type = type;
            TypeInfo = type.GetTypeInfo();
            ConverterType = converterType;
        }

        public Type Type { get; }
        public TypeInfo TypeInfo { get; }
        public Type ConverterType { get; }

        public override string ToString() => ConverterType == null ? Type.Name : $"{Type.Name} Converter={ConverterType.Name}";
    }
}
