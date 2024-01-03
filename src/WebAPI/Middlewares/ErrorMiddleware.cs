using Application.DTOs.Errors;
using Domain.Exceptions;
using System.Net;

namespace WebAPI.Middlewares;

public class ErrorMiddleware : IMiddleware
{
    private readonly ILogger<ErrorMiddleware> _logger;

    public ErrorMiddleware(ILogger<ErrorMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
            return;
        }
        catch (Exception ex)
        {
            ErrorModel error = await GetExceptionResponseAsync(ex);

            await HandleException(httpContext, error);

            return;
        }
    }

    private Task<ErrorModel> GetExceptionResponseAsync(Exception exception)
    {
        int statusCode;

        switch (exception)
        {
            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                break;
            case BadRequestException:
                statusCode = (int)HttpStatusCode.BadRequest;
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        string? message = exception.Message;
        string? trace = exception.StackTrace;

        return Task.FromResult(new ErrorModel()
        {
            Status = statusCode,
            Message = message,
            Trace = trace
        });
    }

    private async Task HandleException(HttpContext httpContext, ErrorModel error)
    {
        _logger.Log(LogLevel.Error, $"----------------------------------------");
        _logger.Log(LogLevel.Error, "Status Code: {status}", error.Status);
        _logger.Log(LogLevel.Error, "Error: {message}", error.Message);
        _logger.Log(LogLevel.Error, "Trace: {trace}", error.Trace);

        httpContext.Response.StatusCode = error.Status;

        var errorView = new ErrorViewModel
        {
            Status = error.Status,
            Message = error.Message!
        };

        await httpContext.Response.WriteAsJsonAsync(errorView);
    }
}
