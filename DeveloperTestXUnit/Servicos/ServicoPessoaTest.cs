using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Servicos;

namespace DeveloperTestXUnit.Servicos;

public class ServicoPessoaTest
{
    private readonly ServicoPessoa _servicoPessoa;
    private readonly Mock<IRepositorioPessoa> _repositorioPessoa;
    private readonly Faker<Pessoa> _fakerPessoa;
    private readonly Faker<PessoaDTO> _fakerPessoaDTO;
    private readonly IMapper _mapper;

    public ServicoPessoaTest(IMapper mapper)
    {
        _mapper = mapper;
        _repositorioPessoa = new Mock<IRepositorioPessoa>();
        _servicoPessoa = new ServicoPessoa(_repositorioPessoa.Object, mapper);
        _fakerPessoa = new Faker<Pessoa>("pt_BR")
            .StrictMode(true)
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.Nome, f => f.Person.FullName)
            .RuleFor(p => p.DataNascimento, f => f.Person.DateOfBirth)
            .RuleFor(p => p.Email, f => f.Person.Email)
            .RuleFor(p => p.Ativo, f => f.Random.Bool());
        _fakerPessoaDTO = new Faker<PessoaDTO>("pt_BR")
            .StrictMode(true)
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.Nome, f => f.Person.FullName)
            .RuleFor(p => p.DataNascimento, f => f.Person.DateOfBirth)
            .RuleFor(p => p.Email, f => f.Person.Email)
            .RuleFor(p => p.Ativo, f => f.Random.Bool());
    }

    [Fact]
    public async Task Deve_Criar_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var pessoaDTO = _fakerPessoaDTO.Generate();
        var pessoaMock = _mapper.Map<Pessoa>(pessoaDTO);
        pessoaMock.Id = Guid.NewGuid();

        var resultadoEsperado = _mapper.Map<PessoaDTO>(pessoaMock);

        _repositorioPessoa.Setup(r => r.CriarAsync(It.IsAny<Pessoa>())).ReturnsAsync(pessoaMock);

        // Action
        var resultado = await _servicoPessoa.CriarAsync(pessoaDTO);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public void Deve_Atualizar_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var pessoaDTO = _fakerPessoaDTO.Generate();
        var pessoaMock = _mapper.Map<Pessoa>(pessoaDTO);

        var resultadoEsperado = _mapper.Map<PessoaDTO>(pessoaMock);

        _repositorioPessoa.Setup(r => r.Atualizar(It.IsAny<Pessoa>())).Returns(pessoaMock);

        // Action
        var resultado = _servicoPessoa.Atualizar(pessoaDTO.Id, pessoaDTO);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var pessoaMock = _fakerPessoa.Generate();

        var resultadoEsperado = _mapper.Map<PessoaDTO>(pessoaMock);

        _repositorioPessoa.Setup(r => r.ObterAsync(pessoaMock.Id)).ReturnsAsync(pessoaMock);

        // Action
        var resultado = await _servicoPessoa.ObterAsync(pessoaMock.Id);

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_A_Lista_De_Pessoa_Com_Sucesso()
    {
        // Arrange
        var listaPessoaMock = _fakerPessoa.Generate(3);
        var resultadoEsperado = _mapper.Map<IEnumerable<PessoaDTO>>(listaPessoaMock);

        _repositorioPessoa.Setup(r => r.ListarAsync(null, null)).ReturnsAsync(listaPessoaMock);

        // Action
        var resultado = await _servicoPessoa.Listar();

        // Assert (Fluent)
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Excluir_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositorioPessoa.SetupSequence(r => r.ExcluirAsync(id)).ReturnsAsync(true).ReturnsAsync(false);

        // Action
        var resultado = await _servicoPessoa.ExcluirAsync(id);
        var resultadoChamada2 = await _servicoPessoa.ExcluirAsync(id);

        // Assert (Fluent)
        resultado.Should().BeTrue();
        resultadoChamada2.Should().BeFalse();
    }
}
