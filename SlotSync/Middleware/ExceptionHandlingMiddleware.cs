using System.Text.Json;
using SlotSync.Exceptions;

namespace SlotSync.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteErrorResponse(
                    context,
                    StatusCodes.Status404NotFound,
                    ex.Message
                );
            }
            catch (BadRequestException ex)
            {
                await WriteErrorResponse(
                    context,
                    StatusCodes.Status400BadRequest,
                    ex.Message
                );
            }
            catch (ConflictException ex)
            {
                await WriteErrorResponse(
                    context,
                    StatusCodes.Status409Conflict,
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                await WriteErrorResponse(
                    context,
                    StatusCodes.Status500InternalServerError,
                    ex.Message
                );
            }
        }

        private static async Task WriteErrorResponse(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}