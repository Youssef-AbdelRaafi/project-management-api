using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Application.Common.Models;

public sealed class PaginatedList<T>
{
    private PaginatedList(IReadOnlyCollection<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public IReadOnlyCollection<T> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasNext => PageNumber < TotalPages;

    public bool HasPrevious => PageNumber > PaginationParams.MinPageNumber;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedPageNumber = Math.Max(pageNumber, PaginationParams.MinPageNumber);
        var normalizedPageSize = Math.Clamp(pageSize, PaginationParams.MinPageSize, PaginationParams.MaxPageSize);
        var totalCount = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((normalizedPageNumber - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, normalizedPageNumber, normalizedPageSize, totalCount);
    }
}
