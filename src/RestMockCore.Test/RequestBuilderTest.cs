using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestMockCore.Test
{
    public class RequestBuilderTest
    {
        private readonly RequestBuilder _requestBuilder;
        public RequestBuilderTest()
        {
            _requestBuilder = new RequestBuilder();
        }

        [Fact]
        public void Handler_Getter_Test()
        {
            Assert.NotNull(_requestBuilder.Handler);
        }

        [Fact]
        public void RequestTest()
        {
            var requestHandler = _requestBuilder.Request("GET", "/test/123/");
            Assert.NotNull(requestHandler);
        }

        [Fact]
        public void RequestTest_with_headers()
        {
            var headers = new Dictionary<string, string> {{"Content-Type", "application/json"}};
            var requestHandler = _requestBuilder.Request("GET", "/test/123/",headers);
            Assert.NotNull(requestHandler);
        }
    }
}
