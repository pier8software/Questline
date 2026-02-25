var builder = DistributedApplication.CreateBuilder(args);

// add mongodb docker container
builder.AddContainer("mongo", "mongo")
    .WithEndpoint(27017, 27017, "tcp", isProxied: false)
    .WithContainerRuntimeArgs("--network", "host");

builder.Build().Run();
