using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Infraestrutura.Extensoes;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Servicos.Externo;
using Microsoft.EntityFrameworkCore;

namespace DeveloperApiTest.Servicos;

public class ServicoCep : ServicoBase<Guid, Endereco, EnderecoDTO>, IServicoCep
{
    private readonly IRepositorioEndereco _repositorioEndereco;
    private readonly IViaCepServico _viaCepServico;
    private readonly IMapper _mapper;
    public ServicoCep(IViaCepServico viaCepServico, IRepositorioEndereco repositorioEndereco, IMapper mapper) : base(repositorioEndereco, mapper)
    {
        _repositorioEndereco = repositorioEndereco;
        _viaCepServico = viaCepServico;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EnderecoDTO>> ObterCepPeloEndereco(string uf, string cidade, string rua)
    {
        var resultados = await _repositorioEndereco.ListarAsync(c =>
            c.UF.ToLower().Equals(uf.ToLower()) &&
            EF.Functions.Like(c.Cidade.ToLower(), $"%{cidade.ToLower()}%") &&
            EF.Functions.Like(c.Logradouro.ToLower(), $"%{rua.ToLower()}%"));

        if (resultados != null && resultados.Any())
        {
            var resultadoFinalDb = _mapper.Map<IEnumerable<EnderecoDTO>>(resultados);
            return resultadoFinalDb;
        }

        // Consulta VIACEP
        var resultado = await _viaCepServico.ObterCepPeloEndereco(uf, cidade, rua);
        ValidarResultadoRequisicao(resultado);
        var resultadoFinalApi = _mapper.Map<IEnumerable<EnderecoDTO>>(resultado.Content!);
        var resultadoFinalBanco = new List<EnderecoDTO>();

        foreach (var registro in resultadoFinalApi)
        {
            var enderecoCriado = await Criar(registro);
            resultadoFinalBanco.Add(enderecoCriado);
        }

        return resultadoFinalBanco;
    }

    public async Task<EnderecoDTO> ObterEnderecoPeloCep(string cep)
    {
        var resultados = await _repositorioEndereco.ListarAsync(c => c.Cep.Equals(cep));
        if (resultados != null && resultados.Any())
        {
            var resultadoFinalDb = _mapper.Map<EnderecoDTO>(resultados.First());
            return resultadoFinalDb;
        }

        // Consulta VIACEP
        var resultado = await _viaCepServico.ObterEnderecoPeloCep(cep);
        ValidarResultadoRequisicao(resultado);
        var resultadoFinal = _mapper.Map<EnderecoDTO>(resultado.Content!);

        return await Criar(resultadoFinal);
    }

    private async Task<EnderecoDTO> Criar(EnderecoDTO enderecoDTO)
    {
        var endereco = _mapper.Map<Endereco>(enderecoDTO);
        endereco.Cep = endereco.Cep.ApenasNumeros().ToString();
        var resultado = await _repositorioEndereco.CriarAsync(endereco);
        var resultadoDTO = _mapper.Map<EnderecoDTO>(resultado);
        return resultadoDTO;
    }
}
