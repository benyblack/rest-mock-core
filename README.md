[![NuGet version](https://badge.fury.io/nu/rest-mock-core.svg)](https://badge.fury.io/nu/rest-mock-core)
[![Build Status](https://benyblack.visualstudio.com/rest-mock-core/_apis/build/status/benyblack.rest-mock-core?branchName=main)](https://benyblack.visualstudio.com/rest-mock-core/_build/latest?definitionId=11&branchName=main)

# rest-mock-core
A simple http server for using in test projects which test .net core based projects.

## Problem
When I started to write some tests for a dotnet core app, I realized that many libraries do not work on that platform.
One of my problems was to find an appropriate *HTTP Server Mock* library. Therefore I started to write this project.

## Install
```console
dotnet add package rest-mock-core
```
## Usage
By default, the server will return "It Works!" with a OK status code (200).

```csharp
HttpServer mockServer = new HttpServer();
mockServer.Run();
```
Then you can use any http client sending request to it.
Default url is http://localhost:5000 which its port could be changed on the constructor:

```csharp
HttpClient httpClient = new HttpClient(5001);
```


## More
There are some options to manage requests better:
```csharp
mockServer.Config.Get("/api/v1/product/123").Send("It Really Works!");
mockServer.Config.Post("/api/v2/store/a123b").Send("Failed!", 503);
mockServer.Config.Get("/messages/123").Send(context =>
            {
                context.Response.StatusCode = 200;
                string response = "<h1>Your new message<h1>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                buffer = System.Text.Encoding.UTF8.GetBytes(response);
                context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            });
```
You can use server.Config either before or after server.Run()

For more details please check Test project.
