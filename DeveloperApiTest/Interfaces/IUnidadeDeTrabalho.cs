using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace DeveloperApiTest.Interfaces;

public interface IUnidadeDeTrabalho
{
    /*
    RepositorioBase<TId, TEntidade> ObterRepositorio<TId, TEntidade>()
        where TId : IComparable, IEquatable<TId>
        where TEntidade : EntidadeBase<TId>;
    */

    DbSet<TEntidade> Registros<TId, TEntidade>()
        where TId : IComparable, IEquatable<TId>
        where TEntidade : EntidadeBase<TId>;

    void EfetivarAlteracoes();
}
