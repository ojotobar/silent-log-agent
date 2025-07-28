using SilentLogAgent;
using SilentLogAgent.Extensions;
using SilentLogAgent.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<MetricsServer>();


var host = builder.Build();
host.Run();
