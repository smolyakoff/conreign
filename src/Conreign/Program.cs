using System.Reflection;
using Conreign.Server.Api;
using Conreign.Server.Api.Handler;
using Conreign.Server.Api.Handler.Behaviours;
using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Core.Gameplay;
using MediatR;
using Orleans.Hosting;
using Orleans.Providers;
using Serilog;

var currentAssembly = Assembly.GetExecutingAssembly();
var appBuilder = WebApplication.CreateBuilder(args);
var host = appBuilder.Host;
host.UseSerilog((_, configuration) => { configuration.WriteTo.Console(); });
host.UseOrleans(builder =>
{
    builder.UseLocalhostClustering();
    builder.AddSimpleMessageStreamProvider(StreamConstants.ProviderName);
    builder.AddMemoryGrainStorage("PubSubStore");
    builder.AddMemoryGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
});

var services = appBuilder.Services;
services.AddRouting();
services
    .AddSignalR()
    .AddNewtonsoftJsonProtocol();
services.AddScoped<HandlerContext>();
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DiagnosticsBehaviour<,>));
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ErrorLoggingBehaviour<,>));
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthenticationBehaviour<,>));
services.AddMediatR(config => config.AsScoped(), currentAssembly);
services.AddAutoMapper(currentAssembly);
services.AddSingleton<LobbyGrainOptions>();
services.AddSingleton<GameGrainOptions>();
services.AddSingleton<GameHubCountersCollection>();

var app = appBuilder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(routes => { routes.MapHub<GameHub>("/$/api"); });
app.MapFallbackToFile("index.html");

app.Run();