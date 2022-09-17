using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Infraestrutura.HealthChecks.Dominio;

[ExcludeFromCodeCoverage]
public class HealthCheckItem
{
    public string Descricao { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
