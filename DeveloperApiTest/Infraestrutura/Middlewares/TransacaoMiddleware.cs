using DeveloperApiTest.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Infraestrutura.Middlewares;

[ExcludeFromCodeCoverage]
public class TransacaoMiddleware : IMiddleware
{
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public TransacaoMiddleware(IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        _unidadeDeTrabalho.EfetivarAlteracoes();

        /*
        // Efetiva alteracoes de banco se for algum metodo que permite alteracao
        var metodo = context.Request.Method.ToUpper();
        if (metodo.Equals("POST") ||
            metodo.Equals("PUT") ||
            metodo.Equals("PATCH") ||
            metodo.Equals("DELETE"))
        {
            _unidadeDeTrabalho.EfetivarAlteracoes();
        }
        */
    }
}
