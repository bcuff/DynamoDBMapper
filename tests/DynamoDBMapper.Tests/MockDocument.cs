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
        public MockEnum Enum { get; set; }
        public MockEnum? NullableMockEnum { get; set; }
        [DynamoDBProperty(Converter = typeof(StringReversingDynamoDBConverter))]
        public string ReversedString { get; set; }
        [DynamoDBIgnore]
        public string IgnoredField { get; set; }
        public double[] DoubleArray { get; set; }
    }

    public enum MockEnum : int
    {
        Zero = 0,
        One = 1,
        NegativeOne = -1,
        MaxValue = int.MaxValue,
        MinValue = int.MinValue,
    }
}
