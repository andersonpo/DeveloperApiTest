using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Validadores;

namespace DeveloperTestXUnit.Validadores;

public class ValidadorPessoaTest
{
    private readonly ValidadorPessoa _validadorPessoa;
    private readonly Faker<PessoaDTO> _fakerPessoaDTO;

    public ValidadorPessoaTest()
    {
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
    public void Deve_Validar_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var pessoaDTO = _fakerPessoaDTO.Generate();

        // Action
        var resultado = _validadorPessoa.Validate(pessoaDTO);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Deve_Validar_Uma_Pessoa_Com_Erro_No_Nome_Vazio()
    {
        // Arrange
        var pessoaDTO = _fakerPessoaDTO.Generate();
        pessoaDTO.Nome = string.Empty;

        // Action
        var resultado = _validadorPessoa.Validate(pessoaDTO);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().HaveCount(2); // 2 erros (nome vazio, nome maior que x caracteres)
    }

    [Fact]
    public void Deve_Validar_Uma_Pessoa_Com_Erro_No_Nome_Pequeno()
    {
        // Arrange
        var pessoaDTO = _fakerPessoaDTO.Generate();
        pessoaDTO.Nome = "AA";

        // Action
        var resultado = _validadorPessoa.Validate(pessoaDTO);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().HaveCount(1); // 1 erro (nome maior que x caracteres)
    }

}
