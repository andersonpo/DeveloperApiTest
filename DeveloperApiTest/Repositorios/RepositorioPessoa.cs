using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;

namespace DeveloperApiTest.Repositorios;

public class RepositorioPessoa : RepositorioBase<Guid, Pessoa>, IRepositorioPessoa
{
    public RepositorioPessoa(IUnidadeDeTrabalho unidadeDeTrabalho) : base(unidadeDeTrabalho)
    {
    }
}
