using System;
using JwtAuthProject.Application.Common;
using JwtAuthProject.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthProject.Api.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult HandleError<T>(Result<T> result)
    {
        return result.ErrorType switch
        {
            ErrorType.Validation => BadRequest(result),
            ErrorType.NotFound => NotFound(result),
            ErrorType.Conflict => Conflict(result),
            ErrorType.Unauthorized => Unauthorized(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError,
                result)
        };
    }
}
