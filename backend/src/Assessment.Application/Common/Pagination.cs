namespace Assessment.Application.Common;

public sealed record PageQuery(int Page = 1, int PageSize = 20)
{
    public const int MaxPageSize = 100;

    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize switch
    {
        < 1 => 20,
        > MaxPageSize => MaxPageSize,
        _ => PageSize
    };

    public int Skip => (SafePage - 1) * SafePageSize;
}

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Total, int Page, int PageSize);
