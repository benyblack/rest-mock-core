using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestMockCore.Test
{
    public class RouteTableItemTest
    {
        Dictionary<string, string> _headers;
        Mock<Microsoft.AspNetCore.Http.HttpRequest> _httpRequestMock;
        public RouteTableItemTest()
        {
            _headers = new Dictionary<string, string>();
            _headers.Add("Content-Type", "application/json");
            _headers.Add("testHeader", "TestHeaderValue");

            var headerDictionary = new HeaderDictionary();
            headerDictionary.Add("Content-Type", new StringValues("application/json"));
            headerDictionary.Add("testHeader", new StringValues("TestHeaderValue"));

            _httpRequestMock = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            _httpRequestMock.Setup(request => request.Headers).Returns(headerDictionary);
            _httpRequestMock.Setup(request => request.Method).Returns("GET");
            _httpRequestMock.Setup(request => request.Path).Returns("/test");
            _httpRequestMock.Setup(request => request.QueryString).Returns(new QueryString("?q=123"));
        }

        [Fact]
        public void Constructor_Test()
        {
            RouteTableItem route = new RouteTableItem();
            Assert.NotNull(route.Request);
        }

        [Fact]
        public void IsMatchTest()
        {
            RouteTableItem route = new RouteTableItem("GET", "/test?q=123", _headers);
            Assert.True(route.IsMatch(_httpRequestMock.Object));

        }


        [Fact]
        public void IsMatchTest_NotContains_Headers()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("testHeader", "TestHeaderValue2");
            RouteTableItem route = new RouteTableItem("GET", "/test?q=123", headers);
            Assert.False(route.IsMatch(_httpRequestMock.Object));

        }

        [Fact]
        public void IsMatchTest_NotContains_NullHeaders()
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("testkey", "testvalue");
            RouteTableItem route = new RouteTableItem("GET", "/test?q=123", headers);
            Assert.False(route.IsMatch(_httpRequestMock.Object));

        }
    }
}
