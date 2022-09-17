using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Dominio.Externo;
using DeveloperApiTest.Infraestrutura.Extensoes;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Servicos;
using DeveloperApiTest.Servicos.Externo;
using Refit;
using System.Linq.Expressions;

namespace DeveloperTestXUnit.Servicos;

public class ServicoCepTest
{
    private readonly ServicoCep _servicoCep;
    private readonly Mock<IViaCepServico> _viaCepServico;
    private readonly Mock<IRepositorioEndereco> _repositorioEndereco;
    private readonly Faker<Endereco> _fakerEndereco;
    private readonly Faker<ViaCepEndereco> _fakerViaCepEndereco;
    private readonly IMapper _mapper;

    public ServicoCepTest(IMapper mapper)
    {
        _mapper = mapper;
        _viaCepServico = new Mock<IViaCepServico>();
        _repositorioEndereco = new Mock<IRepositorioEndereco>();
        _servicoCep = new ServicoCep(_viaCepServico.Object, _repositorioEndereco.Object, mapper);
        _fakerEndereco = new Faker<Endereco>("pt_BR")
            .StrictMode(true)
            .RuleFor(e => e.Id, f => f.Random.Guid())
            .RuleFor(e => e.Cidade, f => f.Address.City())
            .RuleFor(e => e.Logradouro, f => f.Address.StreetAddress())
            .RuleFor(e => e.Complemento, f => f.Address.SecondaryAddress())
            .RuleFor(e => e.UF, f => f.Address.StateAbbr())
            .RuleFor(e => e.Cep, f => f.Address.ZipCode())
            .RuleFor(e => e.Bairro, f => f.Address.Random.Words(2));
        _fakerViaCepEndereco = new Faker<ViaCepEndereco>("pt_BR")
            .StrictMode(true)
            .RuleFor(v => v.Uf, f => f.Address.StateAbbr())
            .RuleFor(v => v.Bairro, f => f.Random.Words())
            .RuleFor(v => v.Cep, f => f.Address.ZipCode())
            .RuleFor(v => v.Complemento, f => f.Address.SecondaryAddress())
            .RuleFor(v => v.DDD, f => f.Random.Number(10, 99).ToString())
            .RuleFor(v => v.Gia, f => f.Random.Number(0, 1000).ToString())
            .RuleFor(v => v.Ibge, f => f.Random.Number(1000, 9999).ToString())
            .RuleFor(v => v.Localidade, f => f.Address.City())
            .RuleFor(v => v.Logradouro, f => f.Address.StreetAddress())
            .RuleFor(v => v.Siafi, f => f.Random.Number(100, 999).ToString());
    }

    [Fact]
    public async Task Deve_Obter_Um_Endereco_Pelo_Cep_Pelo_ViaCep_Com_Sucesso()
    {
        // Arrange
        var enderecoMock = _fakerEndereco.Generate();
        enderecoMock.Cep = enderecoMock.Cep.ApenasNumeros().ToString();

        List<Endereco>? listaEnderecosMock = null;
        var enderecoDTO = _mapper.Map<EnderecoDTO>(enderecoMock);
        var resultadoEsperado = _mapper.Map<EnderecoDTO>(enderecoMock);

        _repositorioEndereco.Setup(r =>
            r.ListarAsync(It.IsAny<Expression<Func<Endereco, bool>>?>(), It.IsAny<Expression<Func<Endereco, IComparable>>?>()))
            .ReturnsAsync(listaEnderecosMock!);

        _repositorioEndereco.Setup(r => r.CriarAsync(It.IsAny<Endereco>())).ReturnsAsync(enderecoMock);

        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        var enderecoViaCep = _mapper.Map<ViaCepEndereco>(enderecoDTO);
        var apiResponseEndereco = new ApiResponse<ViaCepEndereco>(response, enderecoViaCep, new RefitSettings(), null);

        _viaCepServico.Setup(v => v.ObterEnderecoPeloCep(enderecoMock.Cep)).ReturnsAsync(apiResponseEndereco);

        // Action
        var resultado = await _servicoCep.ObterEnderecoPeloCep(enderecoMock.Cep);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Um_Endereco_Pelo_Cep_Do_Banco_Com_Sucesso()
    {
        // Arrange
        var enderecoMock = _fakerEndereco.Generate();
        enderecoMock.Cep = enderecoMock.Cep.ApenasNumeros().ToString();

        var listaEnderecosMock = new List<Endereco>() { enderecoMock };
        var enderecoDTO = _mapper.Map<EnderecoDTO>(enderecoMock);
        var resultadoEsperado = _mapper.Map<EnderecoDTO>(enderecoMock);

        _repositorioEndereco.Setup(r =>
            r.ListarAsync(It.IsAny<Expression<Func<Endereco, bool>>?>(), It.IsAny<Expression<Func<Endereco, IComparable>>?>()))
            .ReturnsAsync(listaEnderecosMock);

        // Action
        var resultado = await _servicoCep.ObterEnderecoPeloCep(enderecoMock.Cep);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Um_Cep_Pelo_Endereco_Pelo_ViaCep_Com_Sucesso()
    {
        // Arrange
        var enderecoMock = _fakerEndereco.Generate();
        var enderecoMock2 = _fakerEndereco.Generate();
        List<Endereco>? listaEnderecosMock = null;

        var listaViaCepEnderecos = _fakerViaCepEndereco.Generate(2);
        var resultadoEsperado = _mapper.Map<IEnumerable<EnderecoDTO>>(new List<Endereco>() { enderecoMock, enderecoMock2 });

        _repositorioEndereco.Setup(r =>
            r.ListarAsync(It.IsAny<Expression<Func<Endereco, bool>>?>(), It.IsAny<Expression<Func<Endereco, IComparable>>?>()))
            .ReturnsAsync(listaEnderecosMock!);

        _repositorioEndereco.SetupSequence(r => r.CriarAsync(It.IsAny<Endereco>())).ReturnsAsync(enderecoMock).ReturnsAsync(enderecoMock2);

        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        var listaEnderecosViaCep = _mapper.Map<IEnumerable<ViaCepEndereco>>(listaViaCepEnderecos);
        var apiResponseEndereco = new ApiResponse<IEnumerable<ViaCepEndereco>>(response, listaEnderecosViaCep, new RefitSettings(), null);

        _viaCepServico.Setup(v => v.ObterCepPeloEndereco(enderecoMock.UF, enderecoMock.Cidade, enderecoMock.Logradouro)).ReturnsAsync(apiResponseEndereco);

        // Action
        var resultado = await _servicoCep.ObterCepPeloEndereco(enderecoMock.UF, enderecoMock.Cidade, enderecoMock.Logradouro);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Um_Cep_Pelo_Endereco_Do_Banco_Com_Sucesso()
    {
        // Arrange
        var enderecoMock = _fakerEndereco.Generate();
        enderecoMock.Cep = enderecoMock.Cep.ApenasNumeros().ToString();

        var listaEnderecosMock = new List<Endereco>() { enderecoMock };
        var enderecoDTO = _mapper.Map<EnderecoDTO>(enderecoMock);
        var resultadoEsperado = _mapper.Map<IEnumerable<EnderecoDTO>>(listaEnderecosMock);

        _repositorioEndereco.Setup(r =>
            r.ListarAsync(It.IsAny<Expression<Func<Endereco, bool>>?>(), It.IsAny<Expression<Func<Endereco, IComparable>>?>()))
            .ReturnsAsync(listaEnderecosMock);

        // Action
        var resultado = await _servicoCep.ObterCepPeloEndereco(enderecoMock.UF, enderecoMock.Cidade, enderecoMock.Cidade);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

}
