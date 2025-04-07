using System.Data;

namespace SourceName.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    internal Task<IDbConnection> CreateConnection(CancellationToken cancellationToken);
}
