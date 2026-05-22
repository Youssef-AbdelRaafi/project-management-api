namespace ProjectManagement.Application.Common.Models;

public sealed class PaginationParams
{
    public const int MinPageNumber = 1;

    public const int MinPageSize = 1;

    public const int DefaultPageSize = 10;

    public const int MaxPageSize = 100;

    private int _pageNumber = MinPageNumber;
    private int _pageSize = DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Math.Max(value, MinPageNumber);
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, MinPageSize, MaxPageSize);
    }

    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }
}
