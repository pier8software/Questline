var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
    .AddDatabase("questline");

builder.AddDockerfile("content-deployer", "../../")
    .WithArgs("--mode=deploy-content")
    .WithReference(mongo)
    .WaitFor(mongo);

builder.Build().Run();
