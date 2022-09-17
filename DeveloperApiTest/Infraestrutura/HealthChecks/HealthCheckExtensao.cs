using DeveloperApiTest.Infraestrutura.HealthChecks.Dominio;
using DeveloperApiTest.Infraestrutura.HealthChecks.HealthChecksCustomizados;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DeveloperApiTest.Infraestrutura.HealthChecks;

[ExcludeFromCodeCoverage]
public static class HealthCheckExtensao
{
    public static IServiceCollection AdicionarHealthCheckCustomizados(this IServiceCollection services)
    {
        const string HEALTH_CHECK_CUSTOMIZED = "CUSTOM_HC";
        const string HEALTH_CHECK_DEPENDENCE = "DEPENDENCIA";

        services.AddHealthChecks()
            .AddCheck<ApiHealthCheck>(name: "Self", tags: new[] { HEALTH_CHECK_CUSTOMIZED })
            .AddCheck<SqlServerHealthCheck>(name: "SqlServer", tags: new[] { HEALTH_CHECK_CUSTOMIZED, HEALTH_CHECK_DEPENDENCE });

        //configura a pagina de healthcheck
        services.AddHealthChecksUI(setupSettings: setup =>
        {
            //guardar por 2hrs (120 historico / 60s cada)
            setup.SetEvaluationTimeInSeconds(60);
            setup.MaximumHistoryEntriesPerEndpoint(120);

            try
            {
                var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
                if (string.IsNullOrWhiteSpace(urls)) return;

                var urlsArray = urls.Split(';');
                var uris = urlsArray.Select(url => Regex.Replace(url, @"^(?<scheme>https?):\/\/((\+)|(\*)|(0.0.0.0))(?=[\:\/]|$)", "${scheme}://localhost"))
                                .Select(uri => new Uri(uri, UriKind.Absolute)).ToArray();
                var httpEndpoint = uris.FirstOrDefault(uri => uri.Scheme == "http");
                var httpsEndpoint = uris.FirstOrDefault(uri => uri.Scheme == "https");

                // Cria o healthcheck de acordo com o schema da url
                if (httpEndpoint != null) // http
                {
                    setup.AddHealthCheckEndpoint("Infraestrutura HTTP", new UriBuilder(httpEndpoint.Scheme, httpEndpoint.Host, httpEndpoint.Port, "/healthz-all").ToString());
                }
                if (httpsEndpoint != null) // https
                {
                    setup.AddHealthCheckEndpoint("Infraestrutura HTTPS", new UriBuilder(httpsEndpoint.Scheme, httpsEndpoint.Host, httpsEndpoint.Port, "/healthz-all").ToString());
                }
            }
            catch { } //NOSONAR - em caso de falha apenas nao registra a pagina de healthcheck

        }).AddInMemoryStorage(); //salva em memoria o historico

        return services;
    }

    public static void UsarHealthChecks(this IApplicationBuilder app)
    {
        // healthcheck self (apenas a api)
        app.UseHealthChecks("/healthz",
            new HealthCheckOptions()
            {
                // filtra apenas a api
                Predicate = e => "self".Equals(e.Name.ToLowerInvariant()),
                ResponseWriter = async (context, report) =>
                {
                    var result = new
                    {
                        status = report.Status.ToString(),
                        entries = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString()
                        })
                    };
                    await context.Response.WriteAsJsonAsync(result);
                }
            });

        // healthcheck dependencias em formato json
        app.UseHealthChecks("/healthz-json",
            new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    var healthcheckInformation = new HealthcheckInformacao
                    {
                        Nome = "Application HealthChecks",
                        Versao = "V1",
                        Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    };

                    var entries = report.Entries.ToList();

                    entries.ForEach(x =>
                    {
                        healthcheckInformation.Itens.Add(new HealthCheckItem
                        {
                            Nome = x.Key,
                            Descricao = string.IsNullOrWhiteSpace(x.Value.Description) ? string.Empty : x.Value.Description,
                            Status = x.Value.Status.ToString()
                        });
                    });

                    var result = JsonSerializer.Serialize(healthcheckInformation);

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }

            });
    }

    public static void UsarHealthCheckUi(this IApplicationBuilder app)
    {
        // retorna todos os healthcheck da aplicacao
        app.UseHealthChecks("/healthz-all", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Ativa o dashboard (pagina) para a visualização da situação de cada Health Check
        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/monitor";
        });
    }
}
