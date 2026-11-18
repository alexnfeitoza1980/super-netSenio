using Microsoft.AspNetCore.Mvc;
using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.Banking.Api.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result, Func<T, IActionResult>? sucesso = null) =>
        result.IsSuccess
            ? (sucesso?.Invoke(result.Value) ?? new OkObjectResult(result.Value))
            : result.Error.ToProblem();

    public static ObjectResult ToProblem(this Error erro)
    {
        var status = erro.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problema = new ProblemDetails
        {
            Status = status,
            Title = erro.Type.ToString(),
            Detail = erro.Description
        };
        problema.Extensions["code"] = erro.Code;

        return new ObjectResult(problema) { StatusCode = status };
    }
}
