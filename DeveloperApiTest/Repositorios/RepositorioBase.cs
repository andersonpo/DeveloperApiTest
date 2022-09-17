using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DeveloperApiTest.Repositorios;

public class RepositorioBase<TId, TEntidade> : IRepositorioBase<TId, TEntidade>
    where TId : IComparable, IEquatable<TId>
    where TEntidade : EntidadeBase<TId>
{
    private readonly IUnidadeDeTrabalho _unidadeDeTrabalho;

    public RepositorioBase(IUnidadeDeTrabalho unidadeDeTrabalho)
    {
        _unidadeDeTrabalho = unidadeDeTrabalho;
    }

    public virtual TEntidade Atualizar(TEntidade entidade)
    {
        var resultado = _unidadeDeTrabalho.Registros<TId, TEntidade>().Update(entidade);
        return resultado.Entity;
    }

    public virtual async Task<TEntidade> CriarAsync(TEntidade entidade)
    {
        var resultado = await _unidadeDeTrabalho.Registros<TId, TEntidade>().AddAsync(entidade);
        return resultado.Entity;
    }

    public virtual async Task<bool> ExcluirAsync(TId id)
    {
        EntityEntry<TEntidade>? resultado = null;
        var entidade = await ObterAsync(id);
        if (entidade != null)
        {
            resultado = _unidadeDeTrabalho.Registros<TId, TEntidade>().Remove(entidade);

            if (resultado != null)
            {
                return resultado.State == EntityState.Deleted;
            }
        }
        return false;
    }

    public virtual async Task<IEnumerable<TEntidade>> ListarAsync(
        Expression<Func<TEntidade, bool>>? filtro = null,
        Expression<Func<TEntidade, IComparable>>? orderBy = null)
    {
        var resultado = _unidadeDeTrabalho.Registros<TId, TEntidade>().AsQueryable();

        if (filtro != null)
            resultado = resultado.Where(filtro);

        if (orderBy != null)
            resultado = resultado.OrderBy(orderBy);

        // TODO: Validar como deixar async no teste de unidade
        //return await resultado.ToListAsync();
        return resultado.ToList();
    }

    public virtual async Task<TEntidade?> ObterAsync(TId id)
    {
        return await _unidadeDeTrabalho.Registros<TId, TEntidade>()
            .SingleOrDefaultAsync(x => x.Id!.Equals(id));
    }
}
