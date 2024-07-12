using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Exceptions
{
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Detail = exception.Message
            };

            if (exception is ValidationException validationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation error(s)";
                problemDetails.Detail = string.Join(", ", validationException.Errors.Select(e => e.ErrorMessage));
            }
            else if (exception is BusinessException businessException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Business exception";
                problemDetails.Detail = businessException.Message;
            }
            else if (exception is AuthorizationException authorizationException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Authorization exception";
                problemDetails.Detail = authorizationException.Message;
            }
            else
            {
                // For general internal server errors
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal server error";
                problemDetails.Detail = exception.Message;
            }

            var json = System.Text.Json.JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }


        private Task CreateAuthorizationException(HttpContext context, AuthorizationException exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.Unauthorized);

            return context.Response.WriteAsync(new AuthorizationProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Type = "https://example.com/probs/authorization",
                Title = "Authorization exception",
                Detail = exception.Message,
                Instance = ""
            }.ToString());
        }

        private Task CreateBusinessException(HttpContext context, BusinessException exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);

            return context.Response.WriteAsync(new BusinessProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://example.com/probs/business",
                Title = "Business exception",
                Detail = exception.Message,
                Instance = ""
            }.ToString());
        }

        private Task CreateValidationException(HttpContext context, ValidationException exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);
            object errors = exception.Errors;

            return context.Response.WriteAsync(new ValidationProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://example.com/probs/validation",
                Title = "Validation error(s)",
                Detail = "",
                Instance = "",
                Errors = errors
            }.ToString());
        }

        private Task CreateIdentityException(HttpContext context, IdentityException exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.BadRequest);

            return context.Response.WriteAsync(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://example.com/probs/identity",
                Title = "Identity error",
                Detail = exception.Message,
                Instance = ""
            }.ToString());
        }

        private Task CreateInternalException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = Convert.ToInt32(HttpStatusCode.InternalServerError);

            return context.Response.WriteAsync(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://example.com/probs/internal",
                Title = "Internal exception",
                Detail = exception.Message,
                Instance = ""
            }.ToString());
        }
    }

    public class IdentityException : Exception
    {
        public IdentityException(string message) : base(message)
        {
        }
    }
}
