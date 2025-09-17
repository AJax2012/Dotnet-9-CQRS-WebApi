using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Contracts;

public interface IToDosRepository
{
    Task<ToDo?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<ToDo>> GetFilteredAsync(GetToDosFilteredQuery filter, ToDo? cursor, CancellationToken cancellationToken);
    Task<ToDo?> GetByTitleAsync(string title, Guid createdByUserId, CancellationToken cancellationToken);
    Task<int> GetCountAsync(GetToDosFilteredQuery filter, CancellationToken cancellationToken);
    Task<int> CreateAsync(ToDo toDo, CancellationToken cancellationToken);
    Task<int> UpdateAsync(ToDo toDo, CancellationToken cancellationToken);
    Task<int> UpdateOrderAsync(IReadOnlyList<ToDo> toDos, CancellationToken cancellationToken);
    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
