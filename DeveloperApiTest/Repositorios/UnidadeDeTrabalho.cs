using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Repositorios;

[ExcludeFromCodeCoverage]
public class UnidadeDeTrabalho : IUnidadeDeTrabalho, IDisposable
{

    private readonly BancoDeDadosContexto _bancoDeDadosContexto;

    /*
    public RepositorioBase<TId, TEntidade> ObterRepositorio<TId, TEntidade>()
        where TId : IComparable, IEquatable<TId>
        where TEntidade : EntidadeBase<TId>
    {
        return new RepositorioBase<TId, TEntidade>(this);
    }
    */

    public UnidadeDeTrabalho(BancoDeDadosContexto bancoDeDadosContexto)
    {
        _bancoDeDadosContexto = bancoDeDadosContexto;
    }

    public DbSet<TEntidade> Registros<TId, TEntidade>()
        where TId : IComparable, IEquatable<TId>
        where TEntidade : EntidadeBase<TId>
    {
        return _bancoDeDadosContexto.Set<TEntidade>();
    }

    public void Dispose()
    {
        _bancoDeDadosContexto.Dispose();
    }

    public void EfetivarAlteracoes()
    {
        _bancoDeDadosContexto.SaveChanges();
    }
}
