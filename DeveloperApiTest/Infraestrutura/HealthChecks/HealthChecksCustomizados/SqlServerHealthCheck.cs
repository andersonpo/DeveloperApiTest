using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Infraestrutura.HealthChecks.HealthChecksCustomizados;

[ExcludeFromCodeCoverage]
public class SqlServerHealthCheck : IHealthCheck
{
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    private string Description = "Conexão com SqlServer";
    public SqlServerHealthCheck(IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var resultado = _unidadeDeTrabalho.Registros<Guid, Pessoa>().Where(p => p.Id == Guid.Empty);
            if (resultado != null && resultado.Count() == 0)
                return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, Description));
            else
                return Task.FromResult(new HealthCheckResult(HealthStatus.Degraded, Description));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, Description, ex));
        }
    }
}
