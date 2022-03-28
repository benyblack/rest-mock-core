using AutoFixture;
using RestMockCore.Models;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace RestMockCore.Test.Models;
public class RequestHandlerTest
{
    private readonly RequestHandler _requestHandler;
    private readonly RouteTableItem _route;
    private readonly string _testBody;
    private const string CONTENT_TYPE_KEY = "Content-Type";
    private const string CONTENT_TYPE_VAL = "Application/Json";
    public RequestHandlerTest()
    {
        var fixture = new Fixture();
        _testBody = fixture.Create<string>();
        var headers = new Dictionary<string, string>
                {
                    {CONTENT_TYPE_KEY, CONTENT_TYPE_VAL},
                    {fixture.Create<string>(), fixture.Create<string>()}
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
        _requestHandler.Send(_testBody);

        //Assert
        Assert.NotNull(_route.Response);
        Assert.Equal(_testBody, _route.Response.Body);
        Assert.Equal(HttpStatusCode.OK, _route.Response.StatusCode);
        Assert.Null(_route.Response.Headers);
    }

    [Fact]
    public void Send_Should_Work_Correctly_Second()
    {
        //Act
        _requestHandler.Send(_testBody, 503);

        //Assert
        Assert.NotNull(_route.Response);
        Assert.Equal(_testBody, _route.Response.Body);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, _route.Response.StatusCode);
        Assert.Null(_route.Response.Headers);
    }

    [Fact]
    public void Send_Should_Work_Correctly_Third()
    {
        //Arrange
        var headers = new Dictionary<string, string> { { CONTENT_TYPE_KEY, CONTENT_TYPE_VAL } };

        //Act
        _requestHandler.Send(_testBody, 503, headers);

        //Assert
        Assert.NotNull(_route.Response);
        Assert.Equal(_testBody, _route.Response.Body);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, _route.Response.StatusCode);
        Assert.Single(_route.Response.Headers);
        Assert.Equal(CONTENT_TYPE_VAL, _route.Response.Headers[CONTENT_TYPE_KEY]);
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
