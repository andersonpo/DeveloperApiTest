using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Infraestrutura.Middlewares;

[ExcludeFromCodeCoverage]
public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var resultado = new ProblemDetails()
            {
                Detail = RecuperarErroException(ex),
                //Extensions = "",
                Instance = $"{context.Request.Method} {context.Request.Path}",
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro na requisição",
                Type = ex.GetType().ToString()
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(resultado);
        }
    }

    private string RecuperarErroException(Exception ex)
    {
        var resultado = string.Empty;
        if (ex == null)
        {
            return resultado;
        }

        if (ex.InnerException != null)
        {
            resultado += $" {ex.Message}";
            return RecuperarErroException(ex.InnerException);
        }

        resultado += $" {ex.Message}";

        return resultado;
    }
}
