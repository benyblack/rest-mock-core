using System.Collections.Generic;
using System.ComponentModel;
using RestMockCore.Models;
using Xunit;

namespace RestMockCore.Test.Models
{
    public class RequestBuilderTest
    {
        private readonly RequestBuilder _requestBuilder;
        public RequestBuilderTest()
        {
            //Arrange
            _requestBuilder = new RequestBuilder();
        }

        [Fact]
        public void Constructor_Should_Work_Correctly()
        {
            //Assert
            Assert.NotNull(_requestBuilder.Handler);
            Assert.NotNull(_requestBuilder.RouteTable);
        }

        [Fact]
        public void Get_Request_Should_Work_Correctly()
        {
            var requestHandler = _requestBuilder.Get("/test/123/");
            Assert.NotNull(requestHandler);
            Assert.NotNull(requestHandler.RouteTable);
            Assert.Equal( "GET",requestHandler.RouteTable.Request.Method);
            Assert.Equal("/test/123/", requestHandler.RouteTable.Request.Url);
        }

        [Fact]
        public void Put_Request_Should_Work_Correctly()
        {
            var requestHandler = _requestBuilder.Put("/test/123/");
            Assert.NotNull(requestHandler);
            Assert.NotNull(requestHandler.RouteTable);
            Assert.Equal("PUT", requestHandler.RouteTable.Request.Method);
            Assert.Equal("/test/123/", requestHandler.RouteTable.Request.Url);
        }

        [Fact]
        public void Post_Request_Should_Work_Correctly()
        {
            var requestHandler = _requestBuilder.Post("/test/123/");
            Assert.NotNull(requestHandler);
            Assert.NotNull(requestHandler.RouteTable);
            Assert.Equal("POST", requestHandler.RouteTable.Request.Method);
            Assert.Equal("/test/123/", requestHandler.RouteTable.Request.Url);
        }

        [Fact]
        public void Delete_Request_Should_Work_Correctly()
        {
            var requestHandler = _requestBuilder.Delete("/test/123/");
            Assert.NotNull(requestHandler);
            Assert.NotNull(requestHandler.RouteTable);
            Assert.Equal("DELETE", requestHandler.RouteTable.Request.Method);
            Assert.Equal("/test/123/", requestHandler.RouteTable.Request.Url);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("GET")]
        [InlineData("DELETE")]
        [InlineData("PUT")]
        public void Request_Request_Should_Work_Correctly(string method)
        {
            var requestHandler = _requestBuilder.Request(method, "/test/123/");
            Assert.NotNull(requestHandler);
            Assert.NotNull(requestHandler.RouteTable);
            Assert.Equal(method, requestHandler.RouteTable.Request.Method);
            Assert.Equal("/test/123/", requestHandler.RouteTable.Request.Url);
        }

        [Fact]
        public void Request_with_headers_Request_Should_Work_Correctly()
        {
            var headers = new Dictionary<string, string> {{"Content-Type", "application/json"}};
            var requestHandler = _requestBuilder.Request("GET", "/test/123/",headers);
            Assert.NotNull(requestHandler);
            Assert.NotNull(requestHandler.RouteTable);
            Assert.Equal("GET", requestHandler.RouteTable.Request.Method);
            Assert.Equal("/test/123/", requestHandler.RouteTable.Request.Url);
            Assert.Single(requestHandler.RouteTable.Request.Headers);
            Assert.Equal("application/json", requestHandler.RouteTable.Request.Headers["Content-Type"]);
        }
    }
}
