using DeveloperApiTest.Controllers;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Validadores;
using FluentValidation;

namespace DeveloperTestXUnit.Controllers;

public class BaseControllerTest
{
    private readonly BaseController _baseController;
    private readonly ValidadorPessoa _validadorPessoa;
    private readonly Faker<PessoaDTO> _fakerPessoaDTO;

    public BaseControllerTest()
    {
        _baseController = new BaseController();
        _validadorPessoa = new ValidadorPessoa();
        _fakerPessoaDTO = new Faker<PessoaDTO>("pt_BR")
            .StrictMode(true)
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.Nome, f => f.Person.FullName)
            .RuleFor(p => p.DataNascimento, f => f.Person.DateOfBirth)
            .RuleFor(p => p.Email, f => f.Person.Email)
            .RuleFor(p => p.Ativo, f => f.Random.Bool());
    }

    [Fact]
    public void Deve_Retonar_Erro_Quando_Id_Menor_Que_Zero()
    {
        // Arrange

        // Action
        var resultado = _baseController.ValidarId(-10);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Erros.Should().HaveCount(1);
        resultado!.Erros.Should().Satisfy(msg => msg.Equals("Id tem que ser maior que zero."));
    }

    [Fact]
    public void Deve_Validar_Com_Sucesso_Quando_Id_Maior_Que_Zero()
    {
        // Arrange

        // Action
        var resultado = _baseController.ValidarId(10);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void Deve_Retonar_Erro_Quando_Guid_Empty()
    {
        // Arrange

        // Action
        var resultado = _baseController.ValidarId(Guid.Empty);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Erros.Should().HaveCount(1);
        resultado!.Erros.Should().Satisfy(msg => msg.Equals("Id não pode ser zero."));
    }

    [Fact]
    public void Deve_Retonar_Sucesso_Quando_Guid_Ok()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Action
        var resultado = _baseController.ValidarId(id);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async void Deve_ValidadarDTO_Com_Sucesso()
    {
        // Arrange
        var pessoaDTO = _fakerPessoaDTO.Generate();

        // Action
        var resultado = await _baseController.ValidarDTO(_validadorPessoa, pessoaDTO);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async void Deve_ValidadarDTO_Com_Erro()
    {
        // Arrange
        var pessoaDTO = _fakerPessoaDTO.Generate();
        pessoaDTO.Nome = "AA";

        // Action
        var resultado = await _baseController.ValidarDTO(_validadorPessoa, pessoaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Erros.Should().HaveCount(1);
        resultado!.Erros.Should().Satisfy(msg => msg.Equals("Nome tem que ser maior que 3 caracteres."));
    }

    [Fact]
    public async void Deve_Retornar_Nulo_Quando_ValidadarDTO_Com_Validador_Nulo()
    {
        // Arrange
        ValidadorPessoa? validadorPessoa = null;

        // Action
        var resultado = await _baseController.ValidarDTO(validadorPessoa!, new PessoaDTO());

        // Assert
        resultado.Should().BeNull();
    }

}
