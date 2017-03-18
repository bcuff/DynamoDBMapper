using System;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace DynamoDBMapper.Tests
{
    public class StringReversingDynamoDBConverter : IPropertyConverter
    {
        public object FromEntry(DynamoDBEntry entry)
        {
            var result = entry.AsString();
            if (result != null)
            {
                result = new string(result.Reverse().ToArray());
            }
            return result;
        }

        public DynamoDBEntry ToEntry(object value)
        {
            var text = (string)value;
            if (text != null)
            {
                text = new string(text.Reverse().ToArray());
            }
            return new Primitive(text);
        }
    }
}
