using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBMapper.Tests
{
    public class MockDocument
    {
        public bool Boolean { get; set; }
        public bool? NullableBoolean { get; set; }
        public int Int32 { get; set; }
        public string String { get; set; }
        [DynamoDBProperty(Converter = typeof(StringReversingDynamoDBConverter))]
        public string ReversedString { get; set; }
        [DynamoDBIgnore]
        public string IgnoredField { get; set; }
    }
}
