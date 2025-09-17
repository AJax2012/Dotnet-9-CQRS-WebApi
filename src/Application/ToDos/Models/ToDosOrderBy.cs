using NetEscapades.EnumGenerators;

namespace SourceName.Application.ToDos.Models;

[EnumExtensions]
public enum ToDosOrderBy
{
    DisplayOrder,
    Title,
    CreatedAt
}
