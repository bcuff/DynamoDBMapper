using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBMapper
{
    internal class DocumentMapping
    {
        public Func<object, Dictionary<string, AttributeValue>> ToAttributesFunc;
        public Func<Dictionary<string, AttributeValue>, object> ToDocumentFunc;
    }
}
