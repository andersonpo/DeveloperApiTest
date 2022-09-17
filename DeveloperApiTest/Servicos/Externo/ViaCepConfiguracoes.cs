using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Servicos.Externo;

[ExcludeFromCodeCoverage]
public class ViaCepConfiguracoes
{
    public const string NomeConfiguracao = "ViaCep";
    public string BaseAddress { get; set; } = string.Empty;
    public string RouteHealthCheck { get; set; } = string.Empty;
    public short Timeout { get; set; } = 15;
}
