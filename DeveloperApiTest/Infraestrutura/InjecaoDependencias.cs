using DeveloperApiTest.Dominio.Mapeamentos;
using DeveloperApiTest.Dominio.Validadores;
using DeveloperApiTest.Infraestrutura.HealthChecks;
using DeveloperApiTest.Infraestrutura.Middlewares;
using DeveloperApiTest.Infraestrutura.Swagger;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Repositorios;
using DeveloperApiTest.Repositorios.Migracoes;
using DeveloperApiTest.Servicos;
using DeveloperApiTest.Servicos.Externo;
using FluentMigrator.Runner;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Refit;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Infraestrutura;

[ExcludeFromCodeCoverage]
public static class InjecaoDependencias
{

    public static IServiceCollection AdicionarDependencias(this IServiceCollection services, IConfiguration configuration)
    {
        // Fluent Validation
        services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<ValidadorPessoa>());

        // JSON Serializacao
        services.AddControllers().AddJsonOptions(options =>
        {
            var serializerOptions = options.JsonSerializerOptions;
            serializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            serializerOptions.WriteIndented = true;
        });

        // Performance (compressao)
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        // AutoMapper
        services.AddAutoMapper(typeof(Mapeamentos));

        // Middleware
        services.AddScoped<ExceptionMiddleware>();
        services.AddScoped<TransacaoMiddleware>();

        // Unidade de Trabalho (Unit of Work)
        services.AddScoped<IUnidadeDeTrabalho, UnidadeDeTrabalho>();

        // HealthCheck
        services.AdicionarHealthCheckCustomizados();

        //Swagger
        services.AddEndpointsApiExplorer();
        services.AdicionarApiVersoes();
        services.AdicionarSwaggerAutorizacaoJWT(configuration);
        services.AdicionarSwagger(apiVersion: "v1");
        services.AdicionarSwagger(apiVersion: "v1.1");
        //services.AdicionarSwagger(apiVersion: "v2.0");

        // Conexao Banco (string)
        var connectionString = configuration.GetConnectionString("SqlServerPrincipal");

        // Fluent Migrator
        var sp = services.AddFluentMigratorCore()
            .ConfigureRunner(rb =>
                rb.AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CriarTabelaPessoa).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider();

        // Execucao da Migracoes
        var runner = sp.GetRequiredService<IMigrationRunner>();
        if (runner.HasMigrationsToApplyUp())
        {
            runner.MigrateUp();
        }

        // Conexao com banco de dados
        services.AddDbContext<BancoDeDadosContexto>(options =>
        {
            options.UseSqlServer(connectionString);
            options.EnableDetailedErrors(true);
        });

        // Refit (Servicos Externos)
        var settings = new RefitSettings();
        settings.HttpMessageHandlerFactory = () => new HttpClientHandler()
        {
            UseCookies = false
        };

        services.AddRefitClient<IViaCepServico>(settings)
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var cfg = serviceProvider.GetRequiredService<IOptions<ViaCepConfiguracoes>>().Value;
                if (string.IsNullOrWhiteSpace(cfg.BaseAddress)) throw new ArgumentException(nameof(cfg.BaseAddress), "BaseAddress do viacep inválido.");
                client.BaseAddress = new Uri(cfg.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(cfg.Timeout);

                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-API-AppKey", cfg.AppKey);
                //client.DefaultRequestHeaders.TryAddWithoutValidation("X-API-AppToken", cfg.AppToken);
            });

        // Options Pattern
        services.Configure<ViaCepConfiguracoes>(configuration.GetSection(ViaCepConfiguracoes.NomeConfiguracao));

        // Servicos
        services.AddScoped<IServicoPessoa, ServicoPessoa>();
        services.AddScoped<IServicoEndereco, ServicoEndereco>();
        services.AddScoped<IServicoCep, ServicoCep>();

        // Repositorios
        services.AddScoped<IRepositorioPessoa, RepositorioPessoa>();
        services.AddScoped<IRepositorioEndereco, RepositorioEndereco>();

        return services;
    }

    public static WebApplication UsarDependencias(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseResponseCompression();

        // Swagger
        app.UseSwagger();
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UsarSwaggerUIMultiplasVersoes(apiVersionDescriptionProvider);

        // Middlware
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<TransacaoMiddleware>();

        // HealthCheck
        app.UsarHealthChecks();
        app.UsarHealthCheckUi();

        // Cors
        app.UseCors(c =>
        {
            c
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        });

        return app;
    }

}
