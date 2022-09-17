using DeveloperApiTest.Dominio.DTOs;

namespace DeveloperApiTest.Interfaces;
public interface IServicoViaCep
{
    Task<EnderecoDTO> ObterEnderecoPeloCep(string cep);
    Task<IEnumerable<EnderecoDTO>> ObterCepPeloEndereco(string uf, string cidade, string rua);
}
