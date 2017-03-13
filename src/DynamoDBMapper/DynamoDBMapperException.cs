using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDBMapper
{
    public class DynamoDBMapperException : Exception
    {
        public DynamoDBMapperException(string propertyName)
            : base($"Unable to parse document on property {propertyName}")
        {
        }
    }
}
