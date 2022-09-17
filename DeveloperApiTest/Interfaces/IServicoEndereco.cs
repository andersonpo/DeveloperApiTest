using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;

namespace DeveloperApiTest.Interfaces;

public interface IServicoEndereco : IServicoBase<Guid, Endereco, EnderecoDTO>
{
    Task<IEnumerable<EnderecoDTO>> ListarPorUfeCidade(string uf, string cidade);
}
