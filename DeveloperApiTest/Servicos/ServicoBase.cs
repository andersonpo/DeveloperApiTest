using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;

namespace DeveloperApiTest.Servicos;

public class ServicoBase<TId, TEntidade, TEntidadeDTO> : ServicoExternoBase, IServicoBase<TId, TEntidade, TEntidadeDTO>
    where TId : IComparable, IEquatable<TId>
    where TEntidade : EntidadeBase<TId>
    where TEntidadeDTO : DTOBase<TId>
{
    private readonly IRepositorioBase<TId, TEntidade> _repositorioBase;
    private readonly IMapper _mapper;

    public ServicoBase(IRepositorioBase<TId, TEntidade> repositorioBase, IMapper mapper)
    {
        _repositorioBase = repositorioBase;
        _mapper = mapper;
    }

    public TEntidadeDTO? Atualizar(TId id, TEntidadeDTO entidadeDTO)
    {
        var entidadeRepositorio = _mapper.Map<TEntidade>(entidadeDTO);
        entidadeRepositorio.Id = id;
        var resultado = _repositorioBase.Atualizar(entidadeRepositorio);
        var entidadeDTOResultado = _mapper.Map<TEntidadeDTO>(resultado);
        return entidadeDTOResultado;
    }

    public async Task<TEntidadeDTO> CriarAsync(TEntidadeDTO entidadeDTO)
    {
        var entidadeRepositorio = _mapper.Map<TEntidade>(entidadeDTO);
        var resultado = await _repositorioBase.CriarAsync(entidadeRepositorio);
        var entidadeDTOResultado = _mapper.Map<TEntidadeDTO>(resultado);
        return entidadeDTOResultado;
    }

    public async Task<bool> ExcluirAsync(TId id)
    {
        var resultado = await _repositorioBase.ExcluirAsync(id);
        return resultado;
    }

    public async Task<IEnumerable<TEntidadeDTO>> Listar()
    {
        var resultado = await _repositorioBase.ListarAsync();
        var listEntidadeDTOResultado = _mapper.Map<IEnumerable<TEntidadeDTO>>(resultado);
        return listEntidadeDTOResultado;
    }

    public async Task<TEntidadeDTO> ObterAsync(TId id)
    {
        var resultado = await _repositorioBase.ObterAsync(id);
        var entidadeDTO = _mapper.Map<TEntidadeDTO>(resultado);
        return entidadeDTO;
    }
}
