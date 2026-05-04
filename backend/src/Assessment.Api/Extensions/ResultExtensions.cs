using Assessment.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, Func<T, ActionResult<T>> onSuccess) =>
        result.Match(onSuccess, error => error.ToFailureActionResult<T>());

    private static ActionResult<T> ToFailureActionResult<T>(this AppError error)
    {
        IActionResult inner = error.Type switch
        {
            AppErrorType.NotFound when error.Title is not null => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = error.Title,
                Detail = error.Detail,
                Type = $"https://httpstatuses.io/{StatusCodes.Status404NotFound}"
            })
            {
                StatusCode = StatusCodes.Status404NotFound
            },
            AppErrorType.NotFound => new NotFoundResult(),
            AppErrorType.Conflict => new ConflictObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = error.Title ?? "Conflict.",
                Type = $"https://httpstatuses.io/{StatusCodes.Status409Conflict}"
            }),
            AppErrorType.Validation => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = error.Title ?? "Ongeldige aanvraag",
                Detail = error.Detail ?? "Validatie is mislukt.",
                Type = $"https://httpstatuses.io/{StatusCodes.Status400BadRequest}"
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            },
            AppErrorType.BadRequest => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = error.Title ?? "Ongeldige aanvraag",
                Detail = error.Detail,
                Type = $"https://httpstatuses.io/{StatusCodes.Status400BadRequest}"
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            },
            _ => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Serverfout",
                Type = $"https://httpstatuses.io/{StatusCodes.Status500InternalServerError}"
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };

        return new ActionResult<T>((ActionResult)inner);
    }
}
