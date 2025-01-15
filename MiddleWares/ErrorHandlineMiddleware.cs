// using Konoha.Common;
// using Microsoft.AspNetCore.Http;
// using System;
// using System.Net;
// using System.Text.Json;
// using System.Threading.Tasks;

// namespace Konoha.Middleware
// {
//     public class ErrorHandlingMiddleware
//     {
//         private readonly RequestDelegate _next;

//         public ErrorHandlingMiddleware(RequestDelegate next)
//         {
//             _next = next;
//         }

//         public async Task InvokeAsync(HttpContext context)
//         {
//             try
//             {
//                 await _next(context);
//             }
//             catch (Exception ex)
//             {
//                 await HandleExceptionAsync(context, ex);
//             }
//         }

//         private static Task HandleExceptionAsync(HttpContext context, Exception exception)
//         {
//             int statusCode;
//             string errorMessage;


//             if (exception is KonohaException konohaException)
//             {
//                 statusCode = konohaException.StatusCode;
//                 errorMessage = konohaException.Message;

//             }
//             else
//             {
//                 statusCode = (int)HttpStatusCode.InternalServerError;
//                 errorMessage = "An unexpected error occurred.";

//             }

//             var errorResponse = new KonohaException(statusCode, errorMessage);

//             context.Response.ContentType = "application/json";
//             context.Response.StatusCode = statusCode;

//             var jsonResponse = JsonSerializer.Serialize(errorResponse);
//             return context.Response.WriteAsync(jsonResponse);
//         }
//     }
// }
