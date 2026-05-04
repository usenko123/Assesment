namespace Assessment.Domain.Common;

public enum AppErrorType
{
    NotFound,
    Conflict,
    Validation,
    BadRequest
}

public sealed record AppError(AppErrorType Type, string? Title = null, string? Detail = null);
