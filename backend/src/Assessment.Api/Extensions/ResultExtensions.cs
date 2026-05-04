using Assessment.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        Func<T, ActionResult<T>> onSuccess) =>
        result.Match(onSuccess, error => controller.ToFailure<T>(error));

    private static ActionResult<T> ToFailure<T>(this ControllerBase controller, AppError error)
    {
        var (status, defaultTitle) = error.Type switch
        {
            AppErrorType.NotFound => (StatusCodes.Status404NotFound, "Niet gevonden"),
            AppErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict."),
            AppErrorType.Validation => (StatusCodes.Status400BadRequest, "Validatie is mislukt."),
            AppErrorType.BadRequest => (StatusCodes.Status400BadRequest, "Ongeldige aanvraag"),
            _ => (StatusCodes.Status500InternalServerError, "Serverfout")
        };

        return controller.Problem(
            statusCode: status,
            title: error.Title ?? defaultTitle,
            detail: error.Detail,
            type: $"https://httpstatuses.io/{status}");
    }
}
