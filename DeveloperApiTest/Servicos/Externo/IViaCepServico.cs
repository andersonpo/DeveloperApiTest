using DeveloperApiTest.Dominio.Externo;
using Refit;

namespace DeveloperApiTest.Servicos.Externo;

public interface IViaCepServico
{
    [Get("/ws/{cep}/json")]
    Task<ApiResponse<ViaCepEndereco>> ObterEnderecoPeloCep([AliasAs("cep")] string cep);
    [Get("/ws/{uf}/{cidade}/{logradouro}/json")]
    Task<ApiResponse<IEnumerable<ViaCepEndereco>>> ObterCepPeloEndereco([AliasAs("uf")] string uf, [AliasAs("cidade")] string cidade, [AliasAs("logradouro")] string logradouro);
}
