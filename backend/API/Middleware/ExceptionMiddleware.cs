using System.Diagnostics;
using System.Text.Json;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Middleware;

/// <summary>
/// Turns exceptions thrown by the services into JSON errors with the right status code,
/// in the same shape ASP.NET Core uses for validation errors:
/// { "title": "...", "status": 409, "errors": { "nationalId": ["..."] } }
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception exception)
        {
            var problem = exception switch
            {
                BadRequestException e => FieldError(400, "One or more validation errors occurred.", e.Field, e.Message),
                ConflictException e => FieldError(409, "Conflict", e.Field, e.Message),
                NotFoundException e => new ProblemDetails { Status = 404, Title = "Not found", Detail = e.Message },

                // Two requests saved the same National ID at the same moment: the unique index stopped the second one.
                DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                    new ProblemDetails { Status = 409, Title = "Conflict", Detail = "This record already exists." },

                _ => new ProblemDetails { Status = 500, Title = "Server error", Detail = "An unexpected error occurred." },
            };

            // Same id as in the server log, so a reported error can be traced.
            problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

            if (problem.Status == 500)
            {
                _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            }

            context.Response.StatusCode = problem.Status!.Value;
            await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json");
        }
    }

    private static ValidationProblemDetails FieldError(int status, string title, string field, string message)
    {
        // camelCase, to match the JSON the client sent: "NationalId" -> "nationalId"
        var key = JsonNamingPolicy.CamelCase.ConvertName(field);
        return new ValidationProblemDetails(new Dictionary<string, string[]> { [key] = [message] })
        {
            Status = status,
            Title = title,
        };
    }
}
