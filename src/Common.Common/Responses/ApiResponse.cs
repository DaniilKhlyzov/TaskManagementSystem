namespace Common.Common.Responses;

public record ApiResponse<T>(bool Succeeded, T? Data, string? Message, string CorrelationId, object? Meta = null);

public static class ApiResponses
{
    public static ApiResponse<T> Ok<T>(T data, string correlationId, object? meta = null, string? message = null)
        => new(true, data, message, correlationId, meta);

    public static ApiResponse<T> Fail<T>(string correlationId, string? message = null)
        => new(false, default, message, correlationId, null);
}

public record PagedResponse<TItem>(IReadOnlyCollection<TItem> Items, int Total, int Page, int PageSize);
