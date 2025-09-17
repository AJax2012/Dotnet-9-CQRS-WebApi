using Dapper;

using SourceName.Application.ToDos.Contracts;
using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;

namespace SourceName.Infrastructure.Persistence.ToDos;

public class ToDosRepository(IDbConnectionFactory connectionFactory) : IToDosRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    private static string BaseQuery => """
        SELECT id AS Id, 
           created_by_user_id AS CreatedByUserId,
           title AS Title,
           display_order AS DisplayOrder,
           is_completed AS IsCompleted,
           created_at AS CreatedAt,
           updated_at AS UpdatedAt
        FROM todos
        /**where**/
        """;

    public async Task<ToDo?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = new SqlBuilder()
            .Where("id = @id", new { id })
            .AddTemplate(BaseQuery);

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        var dbEntity = await connection.QuerySingleOrDefaultAsync<ToDoEntity>(new(query.RawSql, query.Parameters, cancellationToken: cancellationToken));

        return ToDoEntity.ToDomainModel(dbEntity);
    }

    public async Task<IEnumerable<ToDo>> GetFilteredAsync(GetToDosFilteredQuery filter, ToDo? cursor, CancellationToken cancellationToken)
    {
        var builder = new SqlBuilder();

        GetFilteredWhereClause(filter, builder);

        if (cursor is not null)
        {
            (string orderBy, dynamic nextPageToken) = GetNextEntityClause(filter.OrderBy, cursor);
            builder.Where(orderBy, nextPageToken);
        }

        var descendingClause = filter.IsDescending ? "DESC" : "ASC";
        builder.OrderBy($"@orderBy {descendingClause}", new { orderBy = filter.OrderBy });
        builder.AddParameters(new { limit = filter.Limit + 1 });

        var query = builder.AddTemplate($"""
                                         {BaseQuery} 
                                         /**orderby**/
                                         LIMIT @limit
                                         """);

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        var dbEntities = await connection.QueryAsync<ToDoEntity>(new(query.RawSql, query.Parameters, cancellationToken: cancellationToken));
        return dbEntities.Select(ToDoEntity.ToDomainModel)!;
    }

    public async Task<ToDo?> GetByTitleAsync(string title, Guid createdByUserId, CancellationToken cancellationToken)
    {
        var query = new SqlBuilder()
            .Where("title = @title", new { title })
            .Where("created_by_user_id = @createdByUserId", new { createdByUserId })
            .AddTemplate(BaseQuery);

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        var dbEntity = await connection.QuerySingleOrDefaultAsync<ToDoEntity>(new(query.RawSql, query.Parameters, cancellationToken: cancellationToken));

        return ToDoEntity.ToDomainModel(dbEntity);
    }

    public async Task<int> GetCountAsync(GetToDosFilteredQuery filter, CancellationToken cancellationToken)
    {
        var builder = new SqlBuilder();
        GetFilteredWhereClause(filter, builder);

        var query = builder.AddTemplate("""
                            SELECT COUNT(*) FROM todos
                            /**where**/
                            """);

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        return await connection.QuerySingleAsync<int>(new(query.RawSql, query.Parameters, cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(ToDo toDo, CancellationToken cancellationToken)
    {
        const string query = """
                             INSERT INTO todos (id, created_by_user_id, title, display_order, is_completed, created_at, updated_at)
                             VALUES (@id, @createdByUserId, @title, @displayOrder, @isCompleted, @createdAt, @updatedAt)
                             """;

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        return await connection.ExecuteAsync(new(query, new ToDoEntity(toDo), cancellationToken: cancellationToken));
    }

    public async Task<int> UpdateAsync(ToDo toDo, CancellationToken cancellationToken)
    {
        const string query = """
                             UPDATE todos
                             SET title = @title,
                                 display_order = @displayOrder,
                                 is_completed = @isCompleted,
                                 updated_at = @updatedAt
                             WHERE id = @id
                             """;

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        return await connection.ExecuteAsync(new(query, new ToDoEntity(toDo), cancellationToken: cancellationToken));
    }

    public async Task<int> UpdateOrderAsync(IReadOnlyList<ToDo> toDos, CancellationToken cancellationToken)
    {
        var values = toDos.Select(toDo => $"('{toDo.Id}', {toDo.Status.DisplayOrder})").ToList();

        var sql = $"""
                   WITH cte as (
                     SELECT id, display_order
                     FROM (VALUES {string.Join(",", values)}) as t(id, display_order)
                   )
                   UPDATE todos
                   SET display_order = cte.display_order
                   FROM cte
                   WHERE todos.id = cte.id::uuid
                   """;

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        return await connection.ExecuteAsync(new(sql, cancellationToken: cancellationToken));
    }

    public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        const string query = "DELETE FROM todos WHERE id = @id";

        using var connection = await _connectionFactory.CreateConnection(cancellationToken);
        return await connection.ExecuteAsync(new(query, new { id }, cancellationToken: cancellationToken));
    }

    private static void GetFilteredWhereClause(GetToDosFilteredQuery filter, SqlBuilder builder)
    {
        if (filter.IsCompleted.HasValue)
        {
            builder.Where("is_completed = @isCompleted", new { isCompleted = filter.IsCompleted.Value });
        }

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            builder.Where("title LIKE @title", new { title = $"%{filter.Title}%" });
        }

        if (filter.Ids.Count > 0)
        {
            builder.Where("id = ANY(@ids)", new { ids = filter.Ids });
        }
    }

    private static (string orderBy, dynamic nextPageToken) GetNextEntityClause(string orderBy, ToDo cursor) =>
        orderBy switch
        {
            "DisplayOrder" => ("display_order > @nextPageToken", new { nextPageToken = cursor.Status.DisplayOrder }),
            "Title" => ("title > @nextPageToken", new { nextPageToken = cursor.Title.Value }),
            "CreatedAt" => ("created_at > @nextPageToken", new { nextPageToken = cursor.CreatedAt }),
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null)
        };
}
