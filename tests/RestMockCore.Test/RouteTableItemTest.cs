using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
using RestMockCore.Models;
using Xunit;

namespace RestMockCore.Test
{
    public class RouteTableItemTest
    {
        private readonly Dictionary<string, string> _headers;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpRequest> _httpRequestMock;
        public RouteTableItemTest()
        {
            _headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"testHeader", "TestHeaderValue"}
            };

            var headerDictionary = new HeaderDictionary
            {
                {"Content-Type", new StringValues("application/json")},
                {"testHeader", new StringValues("TestHeaderValue")}
            };

            _httpRequestMock = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            _httpRequestMock.Setup(request => request.Headers).Returns(headerDictionary);
            _httpRequestMock.Setup(request => request.Method).Returns("GET");
            _httpRequestMock.Setup(request => request.Path).Returns("/test");
            _httpRequestMock.Setup(request => request.QueryString).Returns(new QueryString("?q=123"));
        }

        [Fact]
        public void Constructor_Test()
        {
            var route = new RouteTableItem();
            Assert.NotNull(route.Request);
        }

        [Fact]
        public void IsMatchTest()
        {
            var route = new RouteTableItem("GET", "/test?q=123", _headers);
            Assert.True(route.IsMatch(_httpRequestMock.Object));

        }


        [Fact]
        public void IsMatchTest_NotContains_Headers()
        {
            var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"testHeader", "TestHeaderValue2"}
            };
            var route = new RouteTableItem("GET", "/test?q=123", headers);
            Assert.False(route.IsMatch(_httpRequestMock.Object));

        }

        [Fact]
        public void IsMatchTest_NotContains_NullHeaders()
        {
            var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"testkey", "testvalue"}
            };
            var route = new RouteTableItem("GET", "/test?q=123", headers);
            Assert.False(route.IsMatch(_httpRequestMock.Object));

        }
    }
}
