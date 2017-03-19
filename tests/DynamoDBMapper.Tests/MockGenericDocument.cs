using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDBMapper.Tests
{
    public class MockGenericDocument<T>
    {
        public T Value { get; set; }
    }
}
