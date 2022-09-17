using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeveloperApiTest.Servicos;

public class ServicoEndereco : ServicoBase<Guid, Endereco, EnderecoDTO>, IServicoEndereco
{
    private readonly IRepositorioEndereco _repositorioEndereco;
    private readonly IMapper _mapper;
    public ServicoEndereco(IRepositorioEndereco repositorioEndereco, IMapper mapper) : base(repositorioEndereco, mapper)
    {
        _repositorioEndereco = repositorioEndereco;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EnderecoDTO>> ListarPorUfeCidade(string uf, string cidade)
    {
        var resultados = await _repositorioEndereco.ListarAsync(e =>
            e.UF.ToUpper().Equals(uf.ToUpper()) &&
            EF.Functions.Like(e.Cidade.ToLower(), $"%{cidade.ToLower()}%"),
            e => e.Logradouro
        );

        if (resultados != null && resultados.Any())
        {
            return _mapper.Map<IEnumerable<EnderecoDTO>>(resultados);
        }

        return new List<EnderecoDTO>();
    }
}
