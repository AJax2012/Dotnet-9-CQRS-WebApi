using FastEndpoints;
using Serilog;

using SourceName.Api.Loaders;
using SourceName.Application.Loaders;
using SourceName.Infrastructure.Loaders;

var builder = WebApplication.CreateBuilder(args);

var logger = builder.AddLogging();
builder.AddIdentityConfiguration(logger)
    .AddServiceDefaults();

builder.Services
    .AddApiModule(builder.Configuration)
    .AddApplicationModule()
    .AddInfrastructureModule(builder.Configuration);

var app = builder.Build();
var isDevelopment = app.Environment.IsDevelopment();

// Configure the HTTP request pipeline.
if (isDevelopment)
{
    app.UseOpenApiUi();
}

app.UseExceptionHandler()
    .UseHttpsRedirection()
    .UseCorsConfiguration(isDevelopment)
    .UseAuthorization()
    .UseAuthentication()
    .UseSerilogRequestLogging();

app.MapDefaultEndpoints();
app.MapFastEndpoints();

app.Run();
