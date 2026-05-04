using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException dbUpdate)
        {
            logger.LogError(exception, "Unhandled exception");
        }
        else if (!IsUniqueViolation(dbUpdate))
        {
            logger.LogError(dbUpdate, "Database update failed");
        }

        var (statusCode, title) = exception switch
        {
            DbUpdateException ex when IsUniqueViolation(ex)
                => (StatusCodes.Status409Conflict, "Resource bestaat al."),
            _ => (StatusCodes.Status500InternalServerError, "Er is iets misgegaan.")
        };

        httpContext.Response.StatusCode = statusCode;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = $"https://httpstatuses.io/{statusCode}"
            },
            Exception = exception
        }).ConfigureAwait(false);

        return true;
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        for (var e = (Exception?)ex; e is not null; e = e.InnerException)
        {
            if (e is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
            {
                return true;
            }
        }

        return false;
    }
}
