using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;

namespace DeveloperApiTest.Interfaces;

public interface IServicoCep : IServicoBase<Guid, Endereco, EnderecoDTO>
{
    Task<EnderecoDTO> ObterEnderecoPeloCep(string cep);
    Task<IEnumerable<EnderecoDTO>> ObterCepPeloEndereco(string uf, string cidade, string rua);
}
