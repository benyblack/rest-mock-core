# rest-mock-core
A simple http server for using in test projects which test .net core based projects.
It tested with Visual Studio 2015 Update 3 and xUnit.

## Problem
When I started to write test for a netcore app, I realized that many of libraries do not work in that platform.
One of my problem is to find an appropriate "Http Server Mock" library. Therefor I started to write this project.

## Usage
By default, the server will return "It Works!" by Ok status code (200).

```
HttpServer mockServer = new HttpServer();
mockServer.Run();
```

## More
There are some options to manage requests better:
```
mockServer.Config.Get("/test/123").Send("It Really Works!");
mockServer.Config.Post("/havij/123").Send("It is not working!", 503);
```

For other details please check Test project.
More options will be added.

**Project is under development and is in its early stage.**
