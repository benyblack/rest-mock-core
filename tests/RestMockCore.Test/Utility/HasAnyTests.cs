using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Sdk;

namespace RestMockCore.Test.Utility
{
    public class HasAnyTests
    {
        [Theory]
        [ClassData(typeof(TestData))]
        public void HasAny_Should_Work_Correctly(bool expected, IEnumerable<object> input)
        {
            Assert.Equal(expected, input.HasAny());
        }


        public class TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {false, null};
                yield return new object[] {false, new List<object>()};
                yield return new object[] {false, new object[] { }};
                yield return new object[] {false, Array.Empty<object>()};
                yield return new object[] {true, new List<object>() {"test", 1, DateTime.Now}};
                yield return new object[] {true, new List<object>() {"test"}};
                yield return new object[] {true, new object[] {"test", 1, DateTime.Now}};
                yield return new object[] {true, new object[] {"test"}};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}