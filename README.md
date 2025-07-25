# Dotnet-9-CQRS-WebApi

Dotnet new template for Dotnet 9 CQRS WebApi with the following features: 

* Aspire
  * OpenTelemetry
  * PostgreSQL database created on startup
  * Runs migrations on startup using FluentMigrator
  * Has Scalar UI Doc links in the Dashboard
  * Seq Dashboard
* FastEndpoints
  * REPR pattern endpoints
  * Commands and Queries
  * Security (etc.)
* Pseudo vertical slice architecture
* Dapper repositories (Npgsql/Postgres)
* Integration Tests
  * WebApplicationFactory pre-configured
  * Verify for testing HTTP responses
  * Fake JWT Tokens for testing
* Unit test projects

**REMINDER**: Update the author name in the Directory.Build.props files.

## How to use

1. Clone the repository
2. Navigate to the root directory of the project
3. Run `dotnet new install .` to install the template
4. Make/navigate to the project directory you want to create
5. Run `dotnet new cqrs` with the following parameter options
   * `--name` - The name of the project:
   * `--IncludeExample` - Includes an example ToDos project
6. Open the project in your favorite IDE
7. Ensure you are using the Aspire https run configuration 
(http does not currently work)

If you include the example project, everything should work as expected. 
You will see the Aspire dashboard open on your browser with the following:

* Postgres
* PgAdmin
* Migrator (runs migrations on startup and shut down)
* Api (main project)

## CQRS Onion Architecture Overview

The basic idea of CQRS in simple terms is a loosely coupled architecture. 
The layers do the following:

- Domain: processes business logic and ensures objects are valid
- Application: maps/translates external requests (API requests) into 
domain models and calls the infrastructure layer's persistence services to persist data.
- Infrastructure: manages external dependencies, such as database 
connections, requests to other services (e.g. API requests to other services, such as an email API), etc.
- Presentation: interacts with inbound requests to the service. 
In this case, a Dotnet 8 Minimal API.

<div align="center">
  <img alt="CQRS Onion" src="https://miro.medium.com/v2/resize:fit:1400/1*8eY3hTiNEWffynPPLqqZmw.jpeg" width="300" />
</div>

In this template, I have also included a Migrator project, 
which is responsible for creating and updating the database schema, 
and 2 Aspire related projects.
