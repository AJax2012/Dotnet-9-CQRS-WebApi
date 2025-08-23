using FastEndpoints;

namespace SourceName.Api.ToDos;

internal sealed class ToDosGroup : Group
{
    public ToDosGroup()
    {
        Configure("todos", ep =>
        {
            ep.Description(x => x.WithTags("To Dos"));
        });
    }
}
