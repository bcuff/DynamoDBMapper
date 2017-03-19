using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamoDBMapper
{
    internal static class TypeHelper
    {
        static readonly HashSet<Type> _numericPrimitives = new HashSet<Type>
        {
            typeof(sbyte), typeof(byte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double),
        };

        public static bool IsNumericPrimitive(Type type) => _numericPrimitives.Contains(type);

        public static MethodInfo GetTryParseMethod(Type type)
        {
            var q = from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    where method.Name == "TryParse" && method.ReturnType == typeof(bool)
                    let p = method.GetParameters()
                    where p.Length == 2
                       && p[0].ParameterType == typeof(string)
                       && p[1].ParameterType.IsByRef
                       && p[1].ParameterType.GetElementType() == type
                       && p[1].IsOut
                    select method;
            return q.FirstOrDefault();
        }
    }
}
