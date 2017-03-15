using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DynamoDBMapper
{
    public class DynamoDBMapperException : Exception
    {
        public static readonly ConstructorInfo Constructor = typeof(DynamoDBMapperException).GetConstructor(new[] { typeof(string) });

        public DynamoDBMapperException(string propertyName)
            : base($"Unable to parse document on property {propertyName}")
        {
        }
    }
}
