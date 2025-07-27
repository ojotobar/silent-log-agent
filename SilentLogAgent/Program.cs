using SilentLogAgent;
using SilentLogAgent.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.ConfigureServices();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
