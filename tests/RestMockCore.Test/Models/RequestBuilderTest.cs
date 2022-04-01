using AutoFixture;
using System;
using System.Collections.Generic;
using Xunit;

namespace RestMockCore.Test.Models;
public class RequestBuilderTest
{
    private readonly RequestBuilder _requestBuilder;
    private readonly string _url;
    private readonly string _responseBody;
    private readonly string _headerKey;
    private readonly string _headerValue;
    public RequestBuilderTest()
    {
        //Arrange
        _requestBuilder = new RequestBuilder();
        var fixture = new Fixture();
        fixture.Inject(new UriScheme("http"));
        _url = fixture.Create<Uri>().PathAndQuery;
        _responseBody = fixture.Create<string>();
        _headerKey = fixture.Create<string>();
        _headerValue = fixture.Create<string>();
    }

    [Fact]
    public void Request_UrlDoesNotSstartWithSlash_ShouldSlashAddedAtTheLeftSide()
    {
        //Arrange
        var url = "test/123";

        //Act
        var result = _requestBuilder.Request("GET", url);

        //Assert
        Assert.Equal("/test/123", result.RouteTable.Request.Url);
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
        // Act
        var requestHandler = _requestBuilder.Get(_url);

        // Assert
        Assert.NotNull(requestHandler);
        Assert.NotNull(requestHandler.RouteTable);
        Assert.Equal("GET", requestHandler.RouteTable.Request.Method);
        Assert.Equal(_url, requestHandler.RouteTable.Request.Url);
    }

    [Fact]
    public void Put_Request_Should_Work_Correctly()
    {
        // Act
        var requestHandler = _requestBuilder.Put(_url);

        // Assert
        Assert.NotNull(requestHandler);
        Assert.NotNull(requestHandler.RouteTable);
        Assert.Equal("PUT", requestHandler.RouteTable.Request.Method);
        Assert.Equal(_url, requestHandler.RouteTable.Request.Url);
    }

    [Fact]
    public void Post_Request_Should_Work_Correctly()
    {
        // Act
        var requestHandler = _requestBuilder.Post(_url);

        // Assert
        Assert.NotNull(requestHandler);
        Assert.NotNull(requestHandler.RouteTable);
        Assert.Equal("POST", requestHandler.RouteTable.Request.Method);
        Assert.Equal(_url, requestHandler.RouteTable.Request.Url);
    }

    [Fact]
    public void Delete_Request_Should_Work_Correctly()
    {
        // Act
        var requestHandler = _requestBuilder.Delete(_url);

        // Assert
        Assert.NotNull(requestHandler);
        Assert.NotNull(requestHandler.RouteTable);
        Assert.Equal("DELETE", requestHandler.RouteTable.Request.Method);
        Assert.Equal(_url, requestHandler.RouteTable.Request.Url);
    }

    [Theory]
    [InlineData("POST")]
    [InlineData("GET")]
    [InlineData("DELETE")]
    [InlineData("PUT")]
    public void Request_Request_Should_Work_Correctly(string method)
    {
        // Act
        var requestHandler = _requestBuilder.Request(method, _url);

        // Assert
        Assert.NotNull(requestHandler);
        Assert.NotNull(requestHandler.RouteTable);
        Assert.Equal(method, requestHandler.RouteTable.Request.Method);
        Assert.Equal(_url, requestHandler.RouteTable.Request.Url);
    }

    [Fact]
    public void Request_with_headers_Request_Should_Work_Correctly()
    {
        // Arrange
        var key = _headerKey;
        var value = _headerValue;
        var headers = new Dictionary<string, string> { { key, value } };

        // Act
        var requestHandler = _requestBuilder.Request("GET", _url, headers);

        // Assert
        Assert.NotNull(requestHandler);
        Assert.NotNull(requestHandler.RouteTable);
        Assert.Equal("GET", requestHandler.RouteTable.Request.Method);
        Assert.Equal(_url, requestHandler.RouteTable.Request.Url);
        Assert.Single(requestHandler.RouteTable.Request.Headers);
        Assert.Equal(value, requestHandler.RouteTable.Request.Headers[key]);
    }

    [Fact]
    public void VerifyAll_ShouldThrowException_WhenAllRoutesAreCalledAndVerifiable()
    {
        // Arrange
        var responseString = _responseBody;
        _requestBuilder.Get(_url).Send(responseString).Verifiable();
        _requestBuilder.Post(_url).Send(responseString).Verifiable();
        _requestBuilder.Put(_url).Send(responseString).Verifiable();

        // Act
        var result = Assert.Throws<Exception>(() => _requestBuilder.VerifyAll());

        // Assert
        Assert.Equal("Route can not be verified", result.Message);
    }

    [Fact]
    public void VerifyAll_Should_Work_WhenAllRoutesAreCalledAndVerifiable()
    {
        // Arrange
        var responseString = _responseBody;
        var routeItem1 = _requestBuilder.Get(_url).Send(responseString).Verifiable();
        var routeItem2 = _requestBuilder.Post(_url).Send(responseString).Verifiable();
        var routeItem3 = _requestBuilder.Put(_url).Send(responseString).Verifiable();

        // Act
        routeItem1.CallCounter = 1;
        routeItem2.CallCounter = 1;
        routeItem3.CallCounter = 1;

        // Assert
        _requestBuilder.VerifyAll();
    }
}
