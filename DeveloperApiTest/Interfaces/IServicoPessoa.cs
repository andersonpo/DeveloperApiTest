using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;

namespace DeveloperApiTest.Interfaces;

public interface IServicoPessoa : IServicoBase<Guid, Pessoa, PessoaDTO>
{
}
