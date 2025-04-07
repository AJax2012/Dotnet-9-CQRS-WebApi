using SourceName.AppHost;

var builder = DistributedApplication.CreateBuilder(args);
var toDosName = "SourceNameToDos";

var postgres = builder.AddPostgres("SourceNamePostgres")
    .WithEnvironment("POSTGRES_DB", toDosName)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();
    
var postgresDatabase = postgres.AddDatabase(toDosName);

var migrator = builder.AddProject<Projects.SourceName_Migrator>("SourceNameMigrator")
    .WithReference(postgresDatabase)
    .WaitFor(postgresDatabase)
    .WithEnvironment("ConnectionStrings:Default", postgresDatabase.Resource.ConnectionStringExpression);

builder.AddProject<Projects.SourceName_Api>("SourceNameApi")
    .WithReference(postgresDatabase)
    .WaitFor(postgresDatabase)
    .WaitForCompletion(migrator)
    .WithEnvironment("ConnectionStrings:Default", postgresDatabase.Resource.ConnectionStringExpression)
    .WithScalarUiDocs()
    .WithSwaggerUiDocs();

builder.Build().Run();
