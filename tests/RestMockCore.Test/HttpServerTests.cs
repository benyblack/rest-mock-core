using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace RestMockCore.Test;

public class HttpServerTests
{
    private HttpServer _mockServer;
    private const string HOST = "localhost";
    private const int PORT = 56789;
    private readonly string _address = $"http://{HOST}:{PORT}";
    private const string CONTENT_TYPE_KEY = "Content-Type";
    private const string CONTENT_TYPE_VAL_TEXT = "text/plain";
    private const string DEFAULT_RESPONSE_BODY = "It Works!";
    private const string NOT_FOUND_RESPONSE_BODY = "Page not found!";
    private const string ROOT = "/";
    private HttpClient _httpClient;
    private readonly Fixture _fixture;

    public HttpServerTests()
    {
        _fixture = new Fixture();
        _httpClient = new HttpClient();
    }

    [Fact]
    public async void Server_With_No_RouteTable_Should_Return_Default_Response()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        var defaultResponse = await _httpClient.GetAsync(_address);
        _mockServer.Dispose();

        //Assert
        Assert.Equal(DEFAULT_RESPONSE_BODY, await defaultResponse.Content.ReadAsStringAsync());
        Assert.Equal(200, (int)defaultResponse.StatusCode);
        Assert.Equal(CONTENT_TYPE_VAL_TEXT, defaultResponse.Content.Headers.GetValues(CONTENT_TYPE_KEY).First());
        Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
    }

    [Fact]
    public async void Server_With_Overridden_Root_Should_Return_Correct_Response()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        var testBody = _fixture.Create<string>();
        _mockServer.Config.Get(ROOT).Send(testBody);
        _mockServer.Run();

        //Act
        var defaultResponse = await _httpClient.GetAsync(_address);
        _mockServer.Dispose();

        //Assert
        Assert.Equal(testBody, await defaultResponse.Content.ReadAsStringAsync());
        Assert.Equal(200, (int)defaultResponse.StatusCode);
        Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
    }

    [Fact]
    public async void Server_Should_Work_With_Configured_Hostname()
    {
        //Arrange
        var hostName = System.Net.Dns.GetHostName();
        _mockServer = new HttpServer(PORT, hostName);
        _mockServer.Run();

        //Act
        var defaultResponse = await _httpClient.GetAsync(_address);
        _mockServer.Dispose();

        //Assert
        Assert.Equal(DEFAULT_RESPONSE_BODY, await defaultResponse.Content.ReadAsStringAsync());
        Assert.Equal(200, (int)defaultResponse.StatusCode);
        Assert.Equal(CONTENT_TYPE_VAL_TEXT, defaultResponse.Content.Headers.GetValues(CONTENT_TYPE_KEY).First());
        Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
    }

    [Fact]
    public async void Request_With_No_Match_RouteTable_Should_Return_PageNotFound()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();
        _mockServer.Config.Get(ROOT).Send(DEFAULT_RESPONSE_BODY);

        //Act
        var responseGet = await _httpClient.GetAsync($"{_address}/{_fixture.Create<string>()}");
        _mockServer.Dispose();

        //Assert
        Assert.Equal(NOT_FOUND_RESPONSE_BODY, await responseGet.Content.ReadAsStringAsync());
        Assert.Equal(404, (int)responseGet.StatusCode);
        Assert.Equal(CONTENT_TYPE_VAL_TEXT, responseGet.Content.Headers.GetValues(CONTENT_TYPE_KEY).First());
        Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
    }

    [Fact]
    public async void Request_Should_Return_PageNotFound_When_No_Route_Has_Been_Added()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        var responseGet = await _httpClient.GetAsync($"{_address}/{_fixture.Create<string>()}");
        _mockServer.Dispose();

        //Assert
        Assert.Equal(NOT_FOUND_RESPONSE_BODY, await responseGet.Content.ReadAsStringAsync());
        Assert.Equal(404, (int)responseGet.StatusCode);
        Assert.Equal(CONTENT_TYPE_VAL_TEXT, responseGet.Content.Headers.GetValues(CONTENT_TYPE_KEY).First());
        Assert.Throws<InvalidOperationException>(() => _httpClient.GetAsync(_address).RunSynchronously());
    }

    [Fact]
    public async void MockServer_Should_SupPORTs_IDisposable()
    {
        //Arrange
        using (_mockServer = new HttpServer(PORT))
        {
            //Act
            _mockServer.Run();
            var defaultResponse = await _httpClient.GetAsync(_address);
            _mockServer.Dispose();

            //Assert
            Assert.Equal(DEFAULT_RESPONSE_BODY, await defaultResponse.Content.ReadAsStringAsync());
            Assert.Equal(200, (int)defaultResponse.StatusCode);
            Assert.Equal(CONTENT_TYPE_VAL_TEXT, defaultResponse.Content.Headers.GetValues(CONTENT_TYPE_KEY).First());
        }
    }

    [Fact]
    public void Add_RouteTableItem_Should_Work_Correctly()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        _mockServer.Config.Get("/test/123").Send("It Really Works!");
        _mockServer.Config.Get("/testAction/123").Send(context =>
        {
            context.Response.StatusCode = 200;
            const string response = "Action Test";
            var buffer = System.Text.Encoding.UTF8.GetBytes(response);
            context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
        });
        _mockServer.Config.Post("/testPost/123").Send("It Not Works!", 503);
        _mockServer.Config.Put("/testPut/123").Send("Internal Server Error!", 500);
        _mockServer.Dispose();

        //Assert
        Assert.NotNull(_mockServer.Config);
        Assert.Equal(4, _mockServer.Config.RouteTable.Count);

        Assert.Equal("/test/123", _mockServer.Config.RouteTable[0].Request.Url);
        Assert.Equal("/testAction/123", _mockServer.Config.RouteTable[1].Request.Url);
        Assert.Equal("/testPost/123", _mockServer.Config.RouteTable[2].Request.Url);
        Assert.Equal("/testPut/123", _mockServer.Config.RouteTable[3].Request.Url);
    }

    [Fact]
    public async void CallingAnEndpoint_ShouldMarkTheRouteIsCalled()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        var url = $"{_fixture.Create<string>()}";
        var routeItem = _mockServer.Config.Get(url).Send(DEFAULT_RESPONSE_BODY);
        var defaultResponse = await _httpClient.GetAsync($"{_address}/{url}");

        _mockServer.Dispose();

        //Assert
        Assert.True(routeItem.IsCalled);
    }

    [Fact]
    public async void Get_Should_Work_Correctly()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        _mockServer.Config.Get("/test/123").Send("It Really Works!");
        var responseGet = await _httpClient.GetAsync($"{_address}/test/123");

        _mockServer.Config.Get("/testAction/123").Send(context =>
        {
            context.Response.StatusCode = 200;
            const string response = "Action Test";
            var buffer = System.Text.Encoding.UTF8.GetBytes(response);
            context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
        });
        var responseGetAction = await _httpClient.GetAsync($"{_address}/testAction/123");
        _mockServer.Dispose();

        //Assert
        Assert.Equal("It Really Works!", await responseGet.Content.ReadAsStringAsync());
        Assert.Equal(200, (int)responseGet.StatusCode);

        Assert.Equal("Action Test", await responseGetAction.Content.ReadAsStringAsync());
        Assert.Equal(200, (int)responseGetAction.StatusCode);
    }

    [Fact]
    public async void Post_Should_Work_Correctly()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        _mockServer.Config.Post("/havij/123").Send("It Not Works!", 503);
        HttpContent postData = new StringContent("{'data':'Test'}");
        var responsePost = await _httpClient.PostAsync($"{_address}/havij/123", postData);
        _mockServer.Dispose();

        //Assert
        Assert.Equal("It Not Works!", await responsePost.Content.ReadAsStringAsync());
        Assert.Equal(503, (int)responsePost.StatusCode);
    }

    [Fact]
    public async void Post_HasHeaders_Should_Work_Correctly()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        Dictionary<string, string> headers = new()
        {
            { "firstheader", "application/json" },
            { "nextHeader", "TestHeaderValue" }
        };
        _mockServer.Config.Request("POST", "/havij/123", headers).Send("It Not Works!", 503);
        HttpContent postData = new StringContent("{'data':'Test'}");

        foreach (var header in headers)
        {
            postData.Headers.Add(header.Key, header.Value);
        }

        var responsePost = await _httpClient.PostAsync($"{_address}/havij/123", postData);
        _mockServer.Dispose();

        //Assert
        Assert.Equal("It Not Works!", await responsePost.Content.ReadAsStringAsync());
        Assert.Equal(503, (int)responsePost.StatusCode);
    }

    [Fact]
    public async void Put_Should_Work_Correctly()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();
        var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"testHeader", "TestHeaderValue"}
            };

        //Act
        _mockServer.Config.Put("/testPut/456").Send("{'status':'isWorking'}", 200, headers);
        HttpContent putData = new StringContent("{'data':'Test'}");
        var putMessage = new HttpRequestMessage(HttpMethod.Put, $"{_address}/testPut/456") { Content = putData };
        var responsePut = await _httpClient.SendAsync(putMessage);
        _mockServer.Dispose();

        //Assert
        Assert.Equal("{'status':'isWorking'}", await responsePut.Content.ReadAsStringAsync());
        Assert.Equal(200, (int)responsePut.StatusCode);
        Assert.Equal("application/json", responsePut.Content.Headers.GetValues("Content-Type").First());
        Assert.Equal("TestHeaderValue", responsePut.Headers.GetValues("testHeader").First());
    }

    [Fact]
    public async void Delete_Should_Work_Correctly()
    {
        //Arrange
        _mockServer = new HttpServer(PORT);
        _mockServer.Run();

        //Act
        _mockServer.Config.Delete("/testDel/456").Send("Deleted", 200);
        var deleteMessage = new HttpRequestMessage(HttpMethod.Delete, $"{_address}/testDel/456");
        var responseDelete = await _httpClient.SendAsync(deleteMessage);
        _mockServer.Dispose();

        //Assert
        Assert.Equal("Deleted", await responseDelete.Content.ReadAsStringAsync());
        Assert.Equal(200, (int)responseDelete.StatusCode);
    }
}
