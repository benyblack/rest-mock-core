# rest-mock-core
A simple http server for using in test projects which test .net core based projects.
It is tested with Visual Studio 2015 Update 3 and xUnit.

## Problem
When I started to write test for a netcore app, I realized that many of libraries do not work in that platform.
One of my problems was to find an appropriate *Http Server Mock* library. Therefor I started to write this project.

## Install
```console
Install-Package rest-mock-core
```
## Usage
By default, the server will return "It Works!" by Ok status code (200).

```csharp
HttpServer mockServer = new HttpServer();
mockServer.Run();
```
Then you can use any http client sending request to it.
Default url is http://localhost:5000 which its port could be changed constructor:

```csharp
HttpClient httpClient = new HttpClient(5001);
```


## More
There are some options to manage requests better:
```csharp
mockServer.Config.Get("/test/123").Send("It Really Works!");
mockServer.Config.Post("/test2/123").Send("It is not working!", 503);
mockServer.Config.Get("/testAction/123").Send(context =>
            {
                context.Response.StatusCode = 200;
                string response = "Action Test";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
                buffer = System.Text.Encoding.UTF8.GetBytes(response);
                context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            });
```
You can use server.Config either before or after server.Run()

For more details please check Test project.
More options will be added ASAP.


