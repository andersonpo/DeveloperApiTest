using DeveloperApiTest.Controllers;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Validadores;
using DeveloperApiTest.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeveloperTestXUnit.Controllers;

public class PessoaControllerTest
{
    private readonly PessoaController _pessoaController;
    private readonly Mock<IServicoPessoa> _pessoaServico;
    private readonly Faker<PessoaDTO> _fakerPessoaDTO;

    public PessoaControllerTest()
    {
        _pessoaServico = new Mock<IServicoPessoa>();
        _pessoaController = new PessoaController(_pessoaServico.Object);
        _fakerPessoaDTO = new Faker<PessoaDTO>("pt_BR")
            .StrictMode(true)
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.Nome, f => f.Person.FullName)
            .RuleFor(p => p.DataNascimento, f => f.Person.DateOfBirth)
            .RuleFor(p => p.Email, f => f.Person.Email)
            .RuleFor(p => p.Ativo, f => f.Random.Bool());
    }

    [Fact]
    public async void Deve_Listar_Pessoa_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerPessoaDTO.Generate(5);

        _pessoaServico.Setup(s => s.Listar()).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _pessoaController.ListarPessoasAsync() as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Obter_Uma_Pessoa_Pelo_Id_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerPessoaDTO.Generate();

        _pessoaServico.Setup(s => s.ObterAsync(It.IsAny<Guid>())).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _pessoaController.ObterPessoaAsync(resultadoEsperado.Id) as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Obter_Uma_Pessoa_Pelo_Id_NotFound()
    {
        // Arrange
        PessoaDTO? pessoa = null;
        _pessoaServico.Setup(s => s.ObterAsync(It.IsAny<Guid>())).ReturnsAsync(pessoa!);

        // Action
        var resultado = await _pessoaController.ObterPessoaAsync(Guid.NewGuid()) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Obter_Uma_Pessoa_Pelo_Id_BadRequest()
    {
        // Arrange
        var erroDto = new ErroDTO(new List<string> { "Id não pode ser zero." });

        // Action
        var resultado = await _pessoaController.ObterPessoaAsync(Guid.Empty) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Criar_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerPessoaDTO.Generate();

        _pessoaServico.Setup(s => s.CriarAsync(It.IsAny<PessoaDTO>())).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _pessoaController.CriarPessoaAsync(resultadoEsperado) as CreatedResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Criar_Uma_Pessoa_Com_Erro()
    {
        // Arrange
        PessoaDTO? pessoa = null;
        var pessoaOK = _fakerPessoaDTO.Generate();

        _pessoaServico.Setup(s => s.CriarAsync(It.IsAny<PessoaDTO>())).ReturnsAsync(pessoa!);

        // Action
        var resultado = await _pessoaController.CriarPessoaAsync(pessoaOK) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Criar_Uma_Pessoa_BadRequest()
    {
        // Arrange
        var erroDto = new ErroDTO(new List<string> { 
            "Nome é obrigatório.",
            "Nome tem que ser maior que 3 caracteres.",
            "Data Nascimento é obrigatório.",
            "Data Nascimento não pode ser menor que 01/01/1900",
            "Email é obrigatório.",
            "Email tem que ser maior que 5 caracteres",
            "Email é inválido." 
        });

        // Action
        var resultado = await _pessoaController.CriarPessoaAsync(new PessoaDTO()) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Atualizar_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerPessoaDTO.Generate();

        _pessoaServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<PessoaDTO>())).Returns(resultadoEsperado);

        // Action
        var resultado = await _pessoaController.AtualizarPessoaAsync(resultadoEsperado.Id, resultadoEsperado) as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Atualizar_Uma_Pessoa_Com_Erro()
    {
        // Arrange
        var resultadoEsperado = _fakerPessoaDTO.Generate();

        PessoaDTO? pessoa = null;
        _pessoaServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<PessoaDTO>())).Returns(pessoa!);

        // Action
        var resultado = await _pessoaController.AtualizarPessoaAsync(resultadoEsperado.Id, resultadoEsperado) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Atualizar_Uma_Pessoa_BadRequest()
    {
        // Arrange
        PessoaDTO? pessoa = null;
        var erroDto = new ErroDTO(new List<string> { 
            "Nome é obrigatório.",
            "Nome tem que ser maior que 3 caracteres.",
            "Data Nascimento é obrigatório.",
            "Data Nascimento não pode ser menor que 01/01/1900",
            "Email é obrigatório.",
            "Email tem que ser maior que 5 caracteres",
            "Email é inválido." 
        });

        _pessoaServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<PessoaDTO>())).Returns(pessoa!);

        // Action
        var resultado = await _pessoaController.AtualizarPessoaAsync(Guid.NewGuid(), new PessoaDTO()) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Atualizar_Uma_Pessoa_Id_BadRequest()
    {
        // Arrange
        PessoaDTO? pessoa = null;
        var erroDto = new ErroDTO(new List<string> { "Id não pode ser zero." });

        _pessoaServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<PessoaDTO>())).Returns(pessoa!);

        // Action
        var resultado = await _pessoaController.AtualizarPessoaAsync(Guid.Empty, new PessoaDTO()) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Excluir_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        _pessoaServico.Setup(s => s.ExcluirAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Action
        var resultado = await _pessoaController.ExcluirPessoaAsync(Guid.NewGuid()) as OkResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async void Deve_Excluir_Uma_Pessoa_Com_Erro()
    {
        // Arrange
        _pessoaServico.Setup(s => s.ExcluirAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Action
        var resultado = await _pessoaController.ExcluirPessoaAsync(Guid.NewGuid()) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Excluir_Uma_Pessoa_Com_Erro_BadRequest()
    {
        // Arrange
        var erroDto = new ErroDTO(new List<string> { "Id não pode ser zero." });
        _pessoaServico.Setup(s => s.ExcluirAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Action
        var resultado = await _pessoaController.ExcluirPessoaAsync(Guid.Empty) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }
}
