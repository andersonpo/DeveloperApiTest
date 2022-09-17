using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;

namespace DeveloperApiTest.Repositorios;

public class RepositorioEndereco : RepositorioBase<Guid, Endereco>, IRepositorioEndereco
{
    public RepositorioEndereco(IUnidadeDeTrabalho unidadeDeTrabalho) : base(unidadeDeTrabalho)
    {
    }
}
