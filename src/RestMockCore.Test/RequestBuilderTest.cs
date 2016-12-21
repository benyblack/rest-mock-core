using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestMockCore.Test
{
    public class RequestBuilderTest
    {
        RequestBuilder requestBuilder;
        public RequestBuilderTest()
        {
            requestBuilder = new RequestBuilder();
        }

        [Fact]
        public void RequestTest()
        {
            IRequestHandler requestHandler = requestBuilder.Request("GET", "/test/123/");
            Assert.NotNull(requestHandler);
        }

        [Fact]
        public void RequestTest_with_headers()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            IRequestHandler requestHandler = requestBuilder.Request("GET", "/test/123/",headers);
            Assert.NotNull(requestHandler);
        }
    }
}
