using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Infraestrutura.HealthChecks.HealthChecksCustomizados;

[ExcludeFromCodeCoverage]
public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HealthCheckResult(
            HealthStatus.Healthy,
            description: "API em execução!"));
    }
}
