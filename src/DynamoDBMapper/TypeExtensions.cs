using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DynamoDBMapper
{
    internal static class TypeExtensions
    {
        public static bool IsNullable(this TypeInfo typeInfo)
        {
            return typeInfo.IsValueType && typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
