namespace RbacSystem.Models.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public PaginationMetadata? Pagination { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> SuccessResponse(T data, PaginationMetadata pagination, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = pagination
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class PaginationMetadata
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public PaginationMetadata Metadata { get; set; } = null!;
}

public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}

public class UserFilterParams : PaginationParams
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
}

public class GroupFilterParams : PaginationParams
{
    public string? GroupName { get; set; }
}

public class RoleFilterParams : PaginationParams
{
    public string? RoleName { get; set; }
}

public class PermissionFilterParams : PaginationParams
{
    public string? PermissionName { get; set; }
}