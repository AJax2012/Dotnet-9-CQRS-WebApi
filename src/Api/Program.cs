using FastEndpoints;
using Serilog;

using SourceName.Api.Loaders;
using SourceName.Application.Loaders;
using SourceName.Infrastructure.Loaders;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLogging()
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
    app.UseScalar();
}

app.UseApiModule(isDevelopment);

app.Run();
