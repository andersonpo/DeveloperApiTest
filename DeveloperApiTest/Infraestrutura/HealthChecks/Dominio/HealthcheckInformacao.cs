using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Infraestrutura.HealthChecks.Dominio;

[ExcludeFromCodeCoverage]
public class HealthcheckInformacao
{
    public string Nome { get; set; } = string.Empty;
    public string Versao { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<HealthCheckItem> Itens { get; set; } = new List<HealthCheckItem>();
}

