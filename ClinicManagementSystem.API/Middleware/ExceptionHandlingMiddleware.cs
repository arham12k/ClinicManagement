using System.Net;
using System.Text.Json;

namespace ClinicManagementSystem.API.Middleware
{
	// Catches unhandled exceptions and returns a consistent JSON error response to the frontend.
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(
			RequestDelegate next,
			ILogger<ExceptionHandlingMiddleware> logger)
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
				_logger.LogError(ex, "Unhandled exception occurred while processing {Path}", context.Request.Path);

				var statusCode = ex switch
				{
					KeyNotFoundException => HttpStatusCode.NotFound,
					UnauthorizedAccessException => HttpStatusCode.Unauthorized,
					ArgumentException => HttpStatusCode.BadRequest,
					InvalidOperationException => HttpStatusCode.BadRequest,
					_ => HttpStatusCode.BadRequest
				};

				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)statusCode;

				var payload = JsonSerializer.Serialize(new
				{
					success = false,
					message = ex.Message
				});

				await context.Response.WriteAsync(payload);
			}
		}
	}
}
