using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamoDBMapper.Tests
{
    internal class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public static readonly IEqualityComparer<byte[]> Instance = new ByteArrayEqualityComparer();

        public bool Equals(byte[] x, byte[] y) => x.SequenceEqual(y);

        public int GetHashCode(byte[] obj)
        {
            int result = 0;
            for (int i = 0; i < obj.Length; ++i)
            {
                result = result ^ (obj[i] << i % 4);
            }
            return result;
        }
    }
}
