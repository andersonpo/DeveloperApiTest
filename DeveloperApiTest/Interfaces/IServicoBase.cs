using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;

namespace DeveloperApiTest.Interfaces;

public interface IServicoBase<TId, TEntidade, TEntidadeDTO>
    where TId : IComparable, IEquatable<TId>
    where TEntidade : EntidadeBase<TId>
    where TEntidadeDTO : DTOBase<TId>
{
    Task<TEntidadeDTO> CriarAsync(TEntidadeDTO entidadeDTO);
    TEntidadeDTO? Atualizar(TId id, TEntidadeDTO entidadeDTO);
    Task<bool> ExcluirAsync(TId id);
    Task<TEntidadeDTO> ObterAsync(TId id);
    Task<IEnumerable<TEntidadeDTO>> Listar();
}
