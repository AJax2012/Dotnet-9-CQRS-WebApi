using SourceName.Application.ToDos.Queries;
using SourceName.Domain.ToDos;

namespace SourceName.Application.ToDos.Contracts;

public interface IToDosRepository
{
    Task<ToDoEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<ToDoEntity>> GetFilteredAsync(GetToDosFilteredQuery filter, ToDoEntity? cursor, CancellationToken cancellationToken);
    Task<ToDoEntity?> GetByTitleAsync(string title, Guid createdByUserId, CancellationToken cancellationToken);
    Task<int> GetCountAsync(GetToDosFilteredQuery filter, CancellationToken cancellationToken);
    Task<int> CreateAsync(ToDoEntity toDo, CancellationToken cancellationToken);
    Task<int> UpdateAsync(ToDoEntity toDo, CancellationToken cancellationToken);
    Task<int> UpdateOrderAsync(IReadOnlyList<ToDoEntity> toDos, CancellationToken cancellationToken);
    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
