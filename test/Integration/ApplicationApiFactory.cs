using System.Diagnostics;

using DotNet.Testcontainers.Builders;

using FluentMigrator.Runner;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using NSubstitute;

using SourceName.Api;
using SourceName.Infrastructure.Persistence;
using SourceName.Infrastructure.Persistence.PostgreSql;
using SourceName.Migrator;
using SourceName.Migrator.Migrations;

using Testcontainers.PostgreSql;

namespace SourceName.Test.Integration;

public class ApplicationApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer =
        new PostgreSqlBuilder()
            .WithDatabase("SourceName")
            .Build();
        
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(_pgContainer.GetConnectionString()));
            
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(_pgContainer.GetConnectionString())
                    .ScanIn(typeof(AddToDosTable).Assembly).For.Migrations())
                .BuildServiceProvider(false);
        });
    }
    
    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();

        var connectionString = _pgContainer.GetConnectionString();
        // Run migrations
        using var scope = Services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public new async Task DisposeAsync()
    {
        await _pgContainer.StopAsync();
    }
}
