namespace EmployeeManagement.BL;

public class EmployeeParameters
{
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    private const int MaxPageSize = 50;
    public int ValidatedPageSize
    {
        get => PageSize > MaxPageSize ? MaxPageSize : PageSize;

    }

    // Sorting
    public string? SortBy { get; set; } // e.g., "FirstName", "Age"
    public bool IsDescending { get; set; } = false;

    // Filtering
    public string? SearchTerm { get; set; } // general search (e.g., name, phone)
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
}
