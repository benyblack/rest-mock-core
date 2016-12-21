# rest-mock-core
A simple http server for using in test project which test .net core based projects.
It tested with Visual Studio 2015 Update 3 and xUnit.

## Start
By default, the server will return "It Works!" by Ok status code (200).

```
HttpServer mockServer = new HttpServer();
mockServer.Run();
```

## More
There is some options to manage requests better:
```
mockServer.Config.Get("/test/123").Send("It Really Works!");
mockServer.Config.Post("/havij/123").Send("It Not Works!", 503);
```

For other details please check Test project.
More options will be added.

**Project is under development and is in its early stage.**