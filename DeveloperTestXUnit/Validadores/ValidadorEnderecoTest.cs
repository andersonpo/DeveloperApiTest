using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Validadores;
using DeveloperApiTest.Infraestrutura.Extensoes;

namespace DeveloperTestXUnit.Validadores;

public class ValidadorEnderecoTest
{
    private readonly ValidadorEndereco _validadorEndereco;
    private readonly Faker<EnderecoDTO> _fakerEnderecoDTO;

    public ValidadorEnderecoTest()
    {
        _validadorEndereco = new ValidadorEndereco();
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
    public void Deve_Validar_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        var enderecoDTO = _fakerEnderecoDTO.Generate();
        enderecoDTO.Cep = enderecoDTO.Cep.ApenasNumeros().ToString();

        // Action
        var resultado = _validadorEndereco.Validate(enderecoDTO);

        // Assert
        resultado.IsValid.Should().BeTrue();
        resultado.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Deve_Validar_Um_Endereco_Com_Erro_No_Cep_Vazio()
    {
        // Arrange
        var enderecoDTO = _fakerEnderecoDTO.Generate();
        enderecoDTO.Cep = string.Empty;

        // Action
        var resultado = _validadorEndereco.Validate(enderecoDTO);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().HaveCount(2); // 2 erros (cep vazio, nome menor que x caracteres)
    }

    [Fact]
    public void Deve_Validar_Um_Endereco_Com_Erro_No_Cep_Pequeno()
    {
        // Arrange
        var enderecoDTO = _fakerEnderecoDTO.Generate();
        enderecoDTO.Cep = "1234567";

        // Action
        var resultado = _validadorEndereco.Validate(enderecoDTO);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().HaveCount(1); // 1 erro (cep menor que x caracteres)
    }

}
