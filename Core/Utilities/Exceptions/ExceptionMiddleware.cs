using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Utilities.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is ValidationException validationException)
        {
            return CreateValidationException(context, validationException);
        }
        else if (exception is BusinessException businessException)
        {
            return CreateBusinessException(context, businessException);
        }
        else if (exception is AuthorizationException authorizationException)
        {
            return CreateAuthorizationException(context, authorizationException);
        }
        else
        {
            return CreateInternalException(context, exception);
        }
    }

    private Task CreateAuthorizationException(HttpContext context, AuthorizationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        var problemDetails = new AuthorizationProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Type = "https://example.com/probs/authorization",
            Title = "Authorization exception",
            Detail = FormatExceptionMessage(exception),
            Instance = "",
            Extensions = { ["ExceptionType"] = exception.GetType().Name }
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private Task CreateBusinessException(HttpContext context, BusinessException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var problemDetails = new BusinessProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://example.com/probs/business",
            Title = "Business exception",
            Detail = FormatExceptionMessage(exception),
            Instance = "",
            Extensions = { ["ExceptionType"] = exception.GetType().Name }
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private Task CreateValidationException(HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        var errors = exception.Errors;

        var errorMessages = errors.Select(error => new
        {
            PropertyName = error.PropertyName,
            ErrorMessage = error.ErrorMessage,
            ErrorCode = error.ErrorCode
        });

        var problemDetails = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://example.com/probs/validation",
            Title = "Validation error(s)",
            Detail = FormatExceptionMessage(exception),
            Instance = "",
            Errors = errorMessages,
            Extensions = { ["ExceptionType"] = exception.GetType().FullName }
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private Task CreateInternalException(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://example.com/probs/internal",
            Title = "Internal exception",
            Extensions = { ["ExceptionType"] = exception.GetType().FullName },
            Detail = FormatExceptionMessage(exception),
            Instance = ""
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }

    private string FormatExceptionMessage(Exception exception)
    {
        string message = exception.Message;

        if (exception.InnerException != null)
        {
            message += " | Inner Exception: " + exception.InnerException.Message.Replace("\r\n", " ").Replace("\n", " ");
        }

        return message;
    }
}

public class IdentityException : Exception
{
    public IdentityException(string message) : base(message)
    {
    }
}
