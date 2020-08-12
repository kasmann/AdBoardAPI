using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AdBoardAPI
{
    public static class GlobalExceptionHandler
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.Use(HandleException);
        }

        private static async Task HandleException(HttpContext httpContext, Func<Task> next)
        {

            var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;

            var problem = new ProblemDetails
            {
                Status = 503,
                Title = feature switch
                {
                    SqlException _ => "Ошибка доступа к базе данных",
                    DbUpdateException _ => "Не удалось сохранить данные",
                    _ => "Внутренняя ошибка сервера"
                }
            };

            var result = JsonConvert.SerializeObject(problem);
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(result);
        }
    }
}
