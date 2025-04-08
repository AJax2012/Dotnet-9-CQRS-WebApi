using SourceName.AppHost;

var builder = DistributedApplication.CreateBuilder(args);
const string pgName = "SourceName";

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_DB", pgName)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();
    
var postgresDatabase = postgres.AddDatabase(pgName);

var migrator = builder.AddProject<Projects.SourceName_Migrator>("migrator")
    .WithReference(postgresDatabase)
    .WaitFor(postgresDatabase)
    .WithEnvironment("ConnectionStrings:Default", postgresDatabase.Resource.ConnectionStringExpression);

builder.AddProject<Projects.SourceName_Api>("api")
    .WithReference(postgresDatabase)
    .WaitFor(postgresDatabase)
    .WaitForCompletion(migrator)
    .WithEnvironment("ConnectionStrings:Default", postgresDatabase.Resource.ConnectionStringExpression)
    .WithScalarUiDocs()
    .WithSwaggerUiDocs();

builder.Build().Run();
