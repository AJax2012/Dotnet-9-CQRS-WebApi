using FluentMigrator.Runner;
using SourceName.Migrator.Migrations;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(configuration.GetConnectionString("Default"))
        .ScanIn(typeof(AddToDosTable).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider(false);


var app = builder.Build();

using var scope = app.Services.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

using (app)
{
    await app.StopAsync(CancellationToken.None);
}
