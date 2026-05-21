using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Application.Common.Models;

/// <summary>
/// Represents one page of data plus pagination metadata.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
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

    /// <summary>
    /// Gets the items in the current page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    /// Gets the current page number.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the current page size.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the total number of items matching the query.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Gets a value indicating whether a next page exists.
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;

    /// <summary>
    /// Gets a value indicating whether a previous page exists.
    /// </summary>
    public bool HasPrevious => PageNumber > PaginationParams.MinPageNumber;

    /// <summary>
    /// Creates a paginated list from an <see cref="IQueryable{T}" /> source.
    /// </summary>
    /// <param name="source">The query source.</param>
    /// <param name="pageNumber">The requested page number.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The paginated query result.</returns>
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
