using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Servicos;
using Microsoft.EntityFrameworkCore;

namespace DeveloperTestXUnit.Servicos;

public class ServicoEnderecoTest
{
    private readonly ServicoEndereco _servicoEndereco;
    private readonly Mock<IRepositorioEndereco> _repositorioEndereco;
    private readonly Faker<Endereco> _fakerEndereco;
    private readonly Faker<EnderecoDTO> _fakerEnderecoDTO;
    private readonly IMapper _mapper;

    public ServicoEnderecoTest(IMapper mapper)
    {
        _mapper = mapper;
        _repositorioEndereco = new Mock<IRepositorioEndereco>();
        _servicoEndereco = new ServicoEndereco(_repositorioEndereco.Object, mapper);
        _fakerEndereco = new Faker<Endereco>("pt_BR")
            .StrictMode(true)
            .RuleFor(e => e.Id, f => f.Random.Guid())
            .RuleFor(e => e.Cidade, f => f.Address.City())
            .RuleFor(e => e.Logradouro, f => f.Address.StreetAddress())
            .RuleFor(e => e.Complemento, f => f.Address.SecondaryAddress())
            .RuleFor(e => e.UF, f => f.Address.StateAbbr())
            .RuleFor(e => e.Cep, f => f.Address.ZipCode())
            .RuleFor(e => e.Bairro, f => f.Address.Random.Words(2));
        _fakerEnderecoDTO = new Faker<EnderecoDTO>("pt_BR")
            .StrictMode(true)
            .RuleFor(e => e.Id, f => f.Random.Guid())
            .RuleFor(e => e.Cidade, f => f.Address.City())
            .RuleFor(e => e.Logradouro, f => f.Address.StreetAddress())
            .RuleFor(e => e.UF, f => f.Address.StateAbbr())
            .RuleFor(e => e.Cep, f => f.Address.ZipCode())
            .RuleFor(e => e.Bairro, f => f.Address.Random.Words(2));
    }

    [Fact]
    public async Task Deve_Criar_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        var enderecoDTO = _fakerEnderecoDTO.Generate();
        var enderecoMock = _mapper.Map<Endereco>(enderecoDTO);
        enderecoMock.Id = Guid.NewGuid();

        var resultadoEsperado = _mapper.Map<EnderecoDTO>(enderecoMock);

        _repositorioEndereco.Setup(r => r.CriarAsync(It.IsAny<Endereco>())).ReturnsAsync(enderecoMock);

        // Action
        var resultado = await _servicoEndereco.CriarAsync(enderecoDTO);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public void Deve_Atualizar_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        var enderecoDTO = _fakerEnderecoDTO.Generate();
        var enderecoMock = _mapper.Map<Endereco>(enderecoDTO);

        var resultadoEsperado = _mapper.Map<EnderecoDTO>(enderecoMock);

        _repositorioEndereco.Setup(r => r.Atualizar(It.IsAny<Endereco>())).Returns(enderecoMock);

        // Action
        var resultado = _servicoEndereco.Atualizar(enderecoDTO.Id, enderecoDTO);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        var enderecoMock = _fakerEndereco.Generate();
        var resultadoEsperado = _mapper.Map<EnderecoDTO>(enderecoMock);

        _repositorioEndereco.Setup(r => r.ObterAsync(enderecoMock.Id)).ReturnsAsync(enderecoMock);

        // Action
        var resultado = await _servicoEndereco.ObterAsync(enderecoMock.Id);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_A_Lista_De_Enderecos_Com_Sucesso()
    {
        // Arrange
        var listaEnderecosMock = _fakerEndereco.Generate(5);
        var resultadoEsperado = _mapper.Map<IEnumerable<EnderecoDTO>>(listaEnderecosMock);

        _repositorioEndereco.Setup(r => r.ListarAsync(null, null)).ReturnsAsync(listaEnderecosMock);

        // Action
        var resultado = await _servicoEndereco.Listar();

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Excluir_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioEndereco.SetupSequence(r => r.ExcluirAsync(id)).ReturnsAsync(true).ReturnsAsync(false);

        // Action
        var resultado = await _servicoEndereco.ExcluirAsync(id);
        var resultadoChamada2 = await _servicoEndereco.ExcluirAsync(id);

        // Assert (Fluent)
        resultado.Should().BeTrue();
        resultadoChamada2.Should().BeFalse();
    }

    [Fact]
    public async Task Deve_Obter_Uma_Lista_De_Enderecos_Por_Uf_e_Cidade()
    {
        // Arrange
        var listaEnderecosMock = _fakerEndereco.Generate(5);
        var resultadoEsperado = _mapper.Map<IEnumerable<EnderecoDTO>>(listaEnderecosMock);

        var uf = "SP";
        var cidade = "Campinas";

        _repositorioEndereco.Setup(r =>
            r.ListarAsync(e =>
                e.UF.ToUpper().Equals(uf.ToUpper()) &&
                EF.Functions.Like(e.Cidade.ToLower(), $"%{cidade.ToLower()}%"),
                e => e.Logradouro))
            .ReturnsAsync(listaEnderecosMock);

        // Action
        var resultado = await _servicoEndereco.ListarPorUfeCidade(uf, cidade);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Uma_Lista_Nula_De_Enderecos_Por_Uf_e_Cidade()
    {
        // Arrange
        List<Endereco>? listaEnderecosMock = null;
        var resultadoEsperado = new List<EnderecoDTO>();

        var uf = "SP";
        var cidade = "Campinas";

        _repositorioEndereco.Setup(r =>
            r.ListarAsync(e =>
                e.UF.ToUpper().Equals(uf.ToUpper()) &&
                EF.Functions.Like(e.Cidade.ToLower(), $"%{cidade.ToLower()}%"),
                e => e.Logradouro))
            .ReturnsAsync(listaEnderecosMock);

        // Action
        var resultado = await _servicoEndereco.ListarPorUfeCidade(uf, cidade);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Uma_Lista_Vazia_De_Enderecos_Por_Uf_e_Cidade()
    {
        // Arrange
        var listaEnderecosMock = new List<Endereco>();
        var resultadoEsperado = _mapper.Map<IEnumerable<EnderecoDTO>>(listaEnderecosMock);

        var uf = "SP";
        var cidade = "Campinas";

        _repositorioEndereco.Setup(r =>
            r.ListarAsync(e =>
                e.UF.ToUpper().Equals(uf.ToUpper()) &&
                EF.Functions.Like(e.Cidade.ToLower(), $"%{cidade.ToLower()}%"),
                e => e.Logradouro))
            .ReturnsAsync(listaEnderecosMock);

        // Action
        var resultado = await _servicoEndereco.ListarPorUfeCidade(uf, cidade);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }
}
