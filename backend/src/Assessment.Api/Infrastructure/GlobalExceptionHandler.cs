using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ProblemDetailsFactory problemDetailsFactory,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var isUniqueViolation = exception is DbUpdateException dbUpdate && IsUniqueViolation(dbUpdate);

        if (!isUniqueViolation)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        var (statusCode, title) = isUniqueViolation
            ? (StatusCodes.Status409Conflict, "Resource bestaat al.")
            : (StatusCodes.Status500InternalServerError, "Er is iets misgegaan.");

        httpContext.Response.StatusCode = statusCode;

        var problem = problemDetailsFactory.CreateProblemDetails(
            httpContext,
            statusCode: statusCode,
            title: title,
            type: $"https://httpstatuses.io/{statusCode}");

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
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
