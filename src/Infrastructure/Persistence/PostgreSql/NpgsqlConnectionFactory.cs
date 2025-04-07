using System.Data;

using Npgsql;

namespace SourceName.Infrastructure.Persistence.PostgreSql;

public class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString;
    
    public async Task<IDbConnection> CreateConnection(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
