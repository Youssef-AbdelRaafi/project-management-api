namespace ProjectManagement.Application.Common.Models;

/// <summary>
/// Represents common pagination, search, and sorting inputs.
/// </summary>
public sealed class PaginationParams
{
    /// <summary>
    /// The smallest valid page number.
    /// </summary>
    public const int MinPageNumber = 1;

    /// <summary>
    /// The smallest valid page size.
    /// </summary>
    public const int MinPageSize = 1;

    /// <summary>
    /// Default page size used when the client does not provide one.
    /// </summary>
    public const int DefaultPageSize = 10;

    /// <summary>
    /// Maximum page size allowed for API pagination.
    /// </summary>
    public const int MaxPageSize = 100;

    private int _pageNumber = MinPageNumber;
    private int _pageSize = DefaultPageSize;

    /// <summary>
    /// Gets or sets the requested page number.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Math.Max(value, MinPageNumber);
    }

    /// <summary>
    /// Gets or sets the requested page size.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, MinPageSize, MaxPageSize);
    }

    /// <summary>
    /// Gets or sets the optional search term.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the optional field name used for sorting.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether sorting is descending.
    /// </summary>
    public bool SortDescending { get; set; }
}
