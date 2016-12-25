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

        [Fact]
        public void IsMatchTest()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("testHeader", "TestHeaderValue");

            HeaderDictionary headerDictionary = new HeaderDictionary();
            headerDictionary.Add("Content-Type", new StringValues("application/json"));
            headerDictionary.Add("testHeader", new StringValues("TestHeaderValue"));

            var httpRequestMock = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            httpRequestMock.Setup(request => request.Headers).Returns(headerDictionary);
            httpRequestMock.Setup(request => request.Method).Returns("GET");
            httpRequestMock.Setup(request => request.Path).Returns("/test");
            httpRequestMock.Setup(request => request.QueryString).Returns(new QueryString("?q=123"));

            RouteTableItem route = new RouteTableItem("GET", "/test?q=123", headers);
            Assert.True(route.IsMatch(httpRequestMock.Object));




        }

    }
}
