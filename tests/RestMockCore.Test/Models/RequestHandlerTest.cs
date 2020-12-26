using System.Collections.Generic;
using RestMockCore.Models;
using Xunit;

namespace RestMockCore.Test.Models
{
    public class RequestHandlerTest
    {
        private readonly RequestHandler _requestHandler;
        private readonly RouteTableItem _route;
        public RequestHandlerTest()
        {
            //Arrange
            var headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"},
                    {"Test_Key", "Test_Value"}
                };
            _route = new RouteTableItem("GET", "/test/123/", headers);
            _requestHandler = new RequestHandler(_route);
        }

        [Fact]
        public void Constructor_Should_Wok_Correctly()
        {
            //Assert
            Assert.NotNull(_requestHandler.Response);
        }

        [Fact]
        public void Send_Should_Work_Correctly_First()
        {
            //Act
            _requestHandler.Send("test body");

            //Assert
            Assert.NotNull(_route.Response);
            Assert.Equal("test body", _route.Response.Body);
            Assert.Equal(200, _route.Response.StatusCode);
            Assert.Null(_route.Response.Headers);
        }

        [Fact]
        public void Send_Should_Work_Correctly_Second()
        {
            //Act
            _requestHandler.Send("test body", 503);

            //Assert
            Assert.NotNull(_route.Response);
            Assert.Equal("test body", _route.Response.Body);
            Assert.Equal(503, _route.Response.StatusCode);
            Assert.Null(_route.Response.Headers);
        }

        [Fact]
        public void Send_Should_Work_Correctly_Third()
        {
            //Arrange
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };

            //Act
            _requestHandler.Send("test body", 503, headers);

            //Assert
            Assert.NotNull(_route.Response);
            Assert.Equal("test body", _route.Response.Body);
            Assert.Equal(503, _route.Response.StatusCode);
            Assert.Single(_route.Response.Headers);
            Assert.Equal("application/json", _route.Response.Headers["Content-Type"]);
        }

        [Fact]
        public void Send_Should_Work_Correctly_Fourth()
        {
            //Act
            _requestHandler.Send(x => x.Request.IsHttps = true);

            //Assert
            Assert.NotNull(_route.Response.Handler);
            Assert.Null(_route.Response.Headers);
        }
    }
}
