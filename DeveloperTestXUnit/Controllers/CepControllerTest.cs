using DeveloperApiTest.Controllers;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeveloperTestXUnit.Controllers;

public class CepControllerTest
{
    private readonly CepController _cepController;
    private readonly Mock<IServicoCep> _cepServico;
    private readonly Faker<EnderecoDTO> _fakerEnderecoDTO;

    public CepControllerTest()
    {
        _cepServico = new Mock<IServicoCep>();
        _cepController = new CepController(_cepServico.Object);
        _fakerEnderecoDTO = new Faker<EnderecoDTO>("pt_BR")
            .StrictMode(true)
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.UF, f => f.Address.StateAbbr())
            .RuleFor(p => p.Cidade, f => f.Address.City())
            .RuleFor(p => p.Bairro, f => f.Person.Email)
            .RuleFor(p => p.Cep, f => f.Address.ZipCode())
            .RuleFor(p => p.Logradouro, f => f.Address.StreetName());
    }

    [Fact]
    public async void Deve_Obter_Endereco_Pelo_Cep_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate();

        _cepServico.Setup(s => s.ObterEnderecoPeloCep(It.IsAny<string>())).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _cepController.ObterEnderecoPeloCep(resultadoEsperado.Cep) as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Obter_Endereco_Pelo_Cep_Em_Branco_Com_BadRequest()
    {
        // Arrange

        // Action
        var resultado = await _cepController.ObterEnderecoPeloCep(string.Empty) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado!.Value.Should().BeEquivalentTo(new { message = "Cep é obrigatório." });
    }

    [Fact]
    public async void Deve_Obter_Endereco_Pelo_Cep_Em_Letras_Com_BadRequest()
    {
        // Arrange

        // Action
        var resultado = await _cepController.ObterEnderecoPeloCep("ABCDEFGHI") as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado!.Value.Should().BeEquivalentTo(new { message = "Cep apenas número." });
    }

    [Fact]
    public async void Deve_Obter_Cep_Pelo_Endereco_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate(3);

        _cepServico.Setup(s => s.ObterCepPeloEndereco(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _cepController.ObterCepPeloEndereco("SP", "Campinas", "Avenida xpto") as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public void Deve_Testar_Rota_Swagger()
    {
        // Arrange

        // Action
        var resultado = _cepController.RotaTesteSwagger() as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(new { message = "Sucesso" });
    }

}
