using DeveloperApiTest.Controllers;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Infraestrutura.Extensoes;
using DeveloperApiTest.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DeveloperTestXUnit.Controllers;

public class EnderecoConstrollerTest
{
    private readonly EnderecoController _enderecoController;
    private readonly Mock<IServicoEndereco> _enderecoServico;
    private readonly Faker<EnderecoDTO> _fakerEnderecoDTO;

    public EnderecoConstrollerTest()
    {
        _enderecoServico = new Mock<IServicoEndereco>();
        _enderecoController = new EnderecoController(_enderecoServico.Object);
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
    public async void Deve_Listar_Endereco_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate(5);

        _enderecoServico.Setup(s => s.Listar()).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _enderecoController.ListarEnderecosAsync() as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Obter_Uma_Endereco_Pelo_Id_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate();

        _enderecoServico.Setup(s => s.ObterAsync(It.IsAny<Guid>())).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _enderecoController.ObterEnderecoAsync(resultadoEsperado.Id) as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Obter_Um_Endereco_Pelo_Id_NotFound()
    {
        // Arrange
        EnderecoDTO? endereco = null;
        _enderecoServico.Setup(s => s.ObterAsync(It.IsAny<Guid>())).ReturnsAsync(endereco!);

        // Action
        var resultado = await _enderecoController.ObterEnderecoAsync(Guid.NewGuid()) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Obter_Um_Endereco_Pelo_Id_BadRequest()
    {
        // Arrange
        var erroDto = new ErroDTO(new List<string> { "Id não pode ser zero." });

        // Action
        var resultado = await _enderecoController.ObterEnderecoAsync(Guid.Empty) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Listar_Endereco_Por_Uf_E_Cidade_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate(2);
        _enderecoServico.Setup(s => s.ListarPorUfeCidade(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _enderecoController.ListarEnderecosPorUfECidadeAsync("SP", "Campinas") as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado!.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Criar_Um_Endereco_BadRequest()
    {
        // Arrange
        var erroDto = new ErroDTO(new List<string> {
            "Cep é obrigatório.",
            "Cep tem que ter 8 digitos.",
            "UF é obrigatório.",
            "UF tem que ter 2 caracteres.",
            "Cidade é obrigatório.",
            "Cidade  tem que ser maior que 3 caracteres.",
            "Bairro é obrigatório.",
            "Bairro  tem que ser maior que 3 caracteres.",
            "Logradouro é obrigatório.",
            "Logradouro  tem que ser maior que 3 caracteres."
        });

        // Action
        var resultado = await _enderecoController.CriarEnderecoAsync(new EnderecoDTO()) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Criar_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate();
        resultadoEsperado.Cep = resultadoEsperado.Cep.ApenasNumeros().ToString();
        _enderecoServico.Setup(s => s.CriarAsync(It.IsAny<EnderecoDTO>())).ReturnsAsync(resultadoEsperado);

        // Action
        var resultado = await _enderecoController.CriarEnderecoAsync(resultadoEsperado) as CreatedResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        resultado.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Criar_Um_Endereco_NotFound()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate();
        resultadoEsperado.Cep = resultadoEsperado.Cep.ApenasNumeros().ToString();
        EnderecoDTO? endereco = null;
        _enderecoServico.Setup(s => s.CriarAsync(It.IsAny<EnderecoDTO>())).ReturnsAsync(endereco!);

        // Action
        var resultado = await _enderecoController.CriarEnderecoAsync(resultadoEsperado) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Atualizar_Um_Endereco_BadRequest()
    {
        // Arrange
        EnderecoDTO? endereco = null;
        var erroDto = new ErroDTO(new List<string> {
            "Cep é obrigatório.",
            "Cep tem que ter 8 digitos.",
            "UF é obrigatório.",
            "UF tem que ter 2 caracteres.",
            "Cidade é obrigatório.",
            "Cidade  tem que ser maior que 3 caracteres.",
            "Bairro é obrigatório.",
            "Bairro  tem que ser maior que 3 caracteres.",
            "Logradouro é obrigatório.",
            "Logradouro  tem que ser maior que 3 caracteres."
        });

        _enderecoServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<EnderecoDTO>())).Returns(endereco!);

        // Action
        var resultado = await _enderecoController.AtualizarEnderecoAsync(Guid.NewGuid(), new EnderecoDTO()) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Atualizar_Um_Endereco_Id_BadRequest()
    {
        // Arrange
        EnderecoDTO? endereco = null;
        var erroDto = new ErroDTO(new List<string> { "Id não pode ser zero." });

        _enderecoServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<EnderecoDTO>())).Returns(endereco!);

        // Action
        var resultado = await _enderecoController.AtualizarEnderecoAsync(Guid.Empty, new EnderecoDTO()) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }

    [Fact]
    public async void Deve_Atualizar_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate();
        resultadoEsperado.Cep = resultadoEsperado.Cep.ApenasNumeros().ToString();

        _enderecoServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<EnderecoDTO>())).Returns(resultadoEsperado);

        // Action
        var resultado = await _enderecoController.AtualizarEnderecoAsync(resultadoEsperado.Id, resultadoEsperado) as OkObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        resultado.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async void Deve_Atualizar_Um_Endereco_NotFound()
    {
        // Arrange
        var resultadoEsperado = _fakerEnderecoDTO.Generate();
        resultadoEsperado.Cep = resultadoEsperado.Cep.ApenasNumeros().ToString();
        EnderecoDTO? endereco = null;

        _enderecoServico.Setup(s => s.Atualizar(It.IsAny<Guid>(), It.IsAny<EnderecoDTO>())).Returns(endereco!);

        // Action
        var resultado = await _enderecoController.AtualizarEnderecoAsync(resultadoEsperado.Id, resultadoEsperado) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Excluir_Um_Endereco_Com_Sucesso()
    {
        // Arrange
        _enderecoServico.Setup(s => s.ExcluirAsync(It.IsAny<Guid>())).ReturnsAsync(true);

        // Action
        var resultado = await _enderecoController.ExcluirEnderecoAsync(Guid.NewGuid()) as OkResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async void Deve_Excluir_Um_Endereco_Com_Erro()
    {
        // Arrange
        _enderecoServico.Setup(s => s.ExcluirAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Action
        var resultado = await _enderecoController.ExcluirEnderecoAsync(Guid.NewGuid()) as NotFoundResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Deve_Excluir_Um_Endereco_Com_Erro_BadRequest()
    {
        // Arrange
        var erroDto = new ErroDTO(new List<string> { "Id não pode ser zero." });
        _enderecoServico.Setup(s => s.ExcluirAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Action
        var resultado = await _enderecoController.ExcluirEnderecoAsync(Guid.Empty) as BadRequestObjectResult;

        // Assert
        resultado.Should().NotBeNull();
        resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        resultado.Value.Should().BeEquivalentTo(erroDto);
    }
}
