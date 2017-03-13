using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DynamoDBMapper
{
    internal class AttributeMapping
    {
        public MethodInfo To;
        public MethodInfo From;
    }
}
