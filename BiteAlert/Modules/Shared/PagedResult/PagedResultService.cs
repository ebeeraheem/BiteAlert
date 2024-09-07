using Microsoft.EntityFrameworkCore;

namespace BiteAlert.Modules.Shared.PagedResult;

public class PagedResultService : IPagedResultService
{
    public async Task<PagedResult<T>> GetPagedResultAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize)
        where T : class
    {
        var result = new PagedResult<T>
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = await query.CountAsync(),
            Items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
        };
        return result;
    }
}
