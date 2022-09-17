using DeveloperApiTest.Dominio.Entidade;
using System.Linq.Expressions;

namespace DeveloperApiTest.Interfaces;

public interface IRepositorioBase<TId, TEntidade>
    where TId : IComparable, IEquatable<TId>
    where TEntidade : EntidadeBase<TId>
{
    Task<TEntidade> CriarAsync(TEntidade entidade);
    TEntidade Atualizar(TEntidade entidade);
    Task<bool> ExcluirAsync(TId id);
    Task<TEntidade?> ObterAsync(TId id);
    Task<IEnumerable<TEntidade>> ListarAsync(Expression<Func<TEntidade, bool>>? filtro = null, Expression<Func<TEntidade, IComparable>>? orderBy = null);
}
