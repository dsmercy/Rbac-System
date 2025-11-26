using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RbacSystem.Middleware;

public class PermissionAuthorizationFilter : IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionAuthorizationFilter(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var permissions = user.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        if (!permissions.Contains(_permission))
        {
            context.Result = new ForbidResult();
        }
    }
}

// Middleware for logging requests
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            await _next(context);

            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogInformation(
                "Request {Method} {Path} completed with status {StatusCode} in {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration
            );
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogError(
                ex,
                "Request {Method} {Path} failed after {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                duration
            );
            throw;
        }
    }
}

// Global exception handler middleware
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            success = false,
            message = "An internal server error occurred",
            errors = new[] { exception.Message }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}