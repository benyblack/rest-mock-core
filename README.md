[![NuGet Badge](https://buildstats.info/nuget/rest-mock-core)](https://www.nuget.org/packages/rest-mock-core/)
[![Build Status](https://benyblack.visualstudio.com/rest-mock-core/_apis/build/status/benyblack.rest-mock-core?branchName=main)](https://benyblack.visualstudio.com/rest-mock-core/_build/latest?definitionId=11&branchName=main)
[![Coverage Status](https://coveralls.io/repos/github/benyblack/rest-mock-core/badge.svg?branch=main)](https://coveralls.io/github/benyblack/rest-mock-core?branch=main)
[![codecov](https://codecov.io/gh/benyblack/rest-mock-core/branch/master/graph/badge.svg?token=qvOpsPXtjh)](https://codecov.io/gh/benyblack/rest-mock-core)


# rest-mock-core
A simple http server for using in test projects which test .net core based projects.

- [rest-mock-core](#rest-mock-core)
  - [Problem](#problem)
  - [Install](#install)
  - [Usage](#usage)
  - [Assert](#assert)
  - [More](#more)

## Problem 
When I started to write some tests for a dotnet core app, I realized that many libraries do not work on that platform.
One of my problems was to find an appropriate **HTTP Server Mocking** library. So, I created this project.

## Install

```console
dotnet add package rest-mock-core
```    
## Usage
You can create and run a mock server as below. Default url is http://localhost:5000 The port can be changed in the constructor:
```csharp
HttpServer mockServer = new HttpServer(5001);
mockServer.Run();
```
Then you can use any http client sending request to it.

```csharp
HttpClient httpClient = new HttpClient(5001);
var response = await httpClient.GetAsync("http://localhost:5001/");
```

* If you call the root of server, it will return *"It Works!"* with a OK status code (200). Of course it can be overrided by adding a responose for the root url.

* You can use `server.Config` to manage requests, then server will return configured responses to your requests :
```csharp
mockServer.Config.Get("/api/product/").Send("It Really Works!");
```
* If you call a address which is not configured, you will receive *"Page not found!"* with status code (404).

## Assert

You can mark each RouteItem as Verifiable and on the assert step all can be verified at once:

```csharp
//Arrange
_mockServer = new HttpServer(PORT);
_mockServer.Run();
_mockServer.Config.Get("/api/product/").Send("It Really Works!").Verifiable();
_mockServer.Config.Post("/api/login/").Send("Welcome.").Verifiable();

//Act
var getMessage = new HttpRequestMessage(HttpMethod.Get, $"{_address}/api/product/");
_ = await _httpClient.SendAsync(getMessage);
var postMessage = new HttpRequestMessage(HttpMethod.Post, $"{_address}/api/login/");
_ = await _httpClient.SendAsync(postMessage);

//Assert
_mockServer.Config.VerifyAll();
```
Also it is possible to verify each route item in different ways:
- Simply verify:
```csharp
//Arrange
var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
route.IsVerifiable = true;
route.CallCounter = 1;

//Act & Assert
route.Verify();
```
- Verify by an action:
```csharp
//Arrange
var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
route.IsVerifiable = true;
route.CallCounter = _fixture.Create<int>();

//Act & Assert
route.Verify(x => x > (route.CallCounter - 1));
```
- Verify using Moq Times:
```csharp
//Arrange
var route = new RouteTableItem(GET, URL_WITH_QUERY, _headers);
route.IsVerifiable = true;
route.CallCounter = _fixture.Create<int>();

//Act & Assert
route.Verify(x => Times.AtLeast(route.CallCounter - 1).Validate(x));
```

## More
There are some options to manage the requests easier:
```csharp
mockServer.Config.Get("/api/v1/product/123").Send("It Really Works!");
mockServer.Config.Post("/api/v2/store/a123b").Send("Failed", 503);
mockServer.Config.Delete("/contact/y7eEty9").Send("Done", HttpStatusCode.OK);
mockServer.Config.Put("/branche/northwest1254").Send("updated", 200);
mockServer.Config.Get("/messages/123").Send(context =>
            {
                context.Response.StatusCode = 200;
                string response = "<h1>Your new message<h1>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                buffer = System.Text.Encoding.UTF8.GetBytes(response);
                context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            });
```
Also, it is possible to use other types of 'http method' by using 'Request':
```csharp
mockServer.Config.Request("PATCH", "/api/v2.3/products/3234")
                 .Send("We don't use PATCH here.", HttpStatusCode.MethodNotAllowed);
```

Lastly, headers can be added to the request:
```csharp
Dictionary<string, string> headers = new(){{"user-agent", "IE6"}};
mockServer.Config.Request("PATCH", "/api/v2.3/products/3234", headers)
                 .Send("C'mon man! Really IE6?", HttpStatusCode.MethodNotAllowed);
```

You can use `server.Config` either before or after `server.Run()`

For more details please check Test project.
