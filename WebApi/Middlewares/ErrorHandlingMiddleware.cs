using Application.Exceptions;
using Common.Responses.Wrappers;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace WebApi.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }

        private static async Task HandleException(HttpContext httpContext, Exception ex)
        {
            var response = httpContext.Response;
            response.ContentType = MediaTypeNames.Application.Json;

            //Error error = new();
            var responseWrapper = await ResponseWrapper.FailAsync(ex.Message);

            switch (ex)
            {
                case CustomValidationException vex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    //error.FriendlyErrorMessage = vex.FriendlyErrorMessage;
                    //error.ErrorMessages = vex.ErrorMessages;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    //error.FriendlyErrorMessage = ex.Message;
                    break;
            }

            //var result = JsonSerializer.Serialize(error);
            var result = JsonSerializer.Serialize(responseWrapper);
            await response.WriteAsync(result);
        }
    }
}