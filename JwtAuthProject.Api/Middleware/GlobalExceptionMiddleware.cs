using System;
using System.Net;
using System.Text.Json;

namespace JwtAuthProject.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (System.Exception ex)
        {
            _logger.LogInformation(ex, "Error in Server");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var responce = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Clien Error",
                Detail = ex.Message
            };

            var json = JsonSerializer.Serialize(responce);

            await context.Response.WriteAsync(json);
        }
    }
}