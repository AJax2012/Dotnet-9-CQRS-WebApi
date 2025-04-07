using NetEscapades.EnumGenerators;

namespace SourceName.Contracts.ToDos;

[EnumExtensions]
public enum ToDosOrderBy
{
    DisplayOrder,
    Title,
    CreatedAt
}
