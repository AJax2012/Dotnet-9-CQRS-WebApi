using FastEndpoints;

using SourceName.Api.Loaders;
using SourceName.Application.Loaders;
using SourceName.Infrastructure.Loaders;

var builder = WebApplication.CreateBuilder(args);

var logger = builder.AddLogging();
builder.AddIdentityConfiguration(logger)
    .AddServiceDefaults();

builder.Services
    .AddProblemDetails()
    .AddCorsConfiguration(builder.Configuration)
    .AddFastEndpoints()
    .AddOpenApiDocuments()
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
    .UseAuthentication();

app.MapDefaultEndpoints();
app.MapFastEndpoints();

app.Run();
