using Microsoft.AspNetCore.Http;
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

            var httpRequestMock = new Mock<Microsoft.AspNetCore.Http.HttpRequest>();
            httpRequestMock.Setup(request => request.Headers).Returns(headers as IHeaderDictionary);
            httpRequestMock.Setup(request => request.Method).Returns("GET");
            httpRequestMock.Setup(request => request.Path).Returns("/test");
            httpRequestMock.Setup(request => request.QueryString).Returns(new QueryString("?q=123"));

            RouteTableItem route = new RouteTableItem("GET", "http://localhost:5000/test?q=123", headers);
            Assert.True(route.IsMatch(httpRequestMock.Object));




        }

    }
}
