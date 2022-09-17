using AutoMapper;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Servicos;
using Refit;

namespace DeveloperTestXUnit.Servicos;

public class ServicoExternoBaseTest
{
    private readonly ServicoExternoBase _servicoExternoBase;
    private readonly Mock<IRepositorioEndereco> _repositorioEndereco;
    private readonly Faker<Endereco> _fakerEndereco;

    public ServicoExternoBaseTest(IMapper mapper)
    {
        _repositorioEndereco = new Mock<IRepositorioEndereco>();
        _servicoExternoBase = new ServicoExternoBase();
        _fakerEndereco = new Faker<Endereco>("pt_BR")
            .StrictMode(true)
            .RuleFor(e => e.Id, f => f.Random.Guid())
            .RuleFor(e => e.Cidade, f => f.Address.City())
            .RuleFor(e => e.Logradouro, f => f.Address.StreetAddress())
            .RuleFor(e => e.Complemento, f => f.Address.SecondaryAddress())
            .RuleFor(e => e.UF, f => f.Address.StateAbbr())
            .RuleFor(e => e.Cep, f => f.Address.ZipCode())
            .RuleFor(e => e.Bairro, f => f.Address.Random.Words(2));
    }

    [Fact]
    public void Deve_Validar_Uma_Resposta_Sem_Corpo()
    {
        // Arrange
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        var apiResponseEndereco = new ApiResponse<Endereco>(response, null, new RefitSettings(), null);

        // Action
        Action act = () => _servicoExternoBase.ValidarResultadoRequisicao(apiResponseEndereco, false);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Deve_Gerar_Excecao_Ao_Validar_Uma_Resposta_Com_StatusCode_Nao_Sucesso()
    {
        // Arrange
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        var apiResponseEndereco = new ApiResponse<Endereco>(response, null, new RefitSettings(), null);

        // Action
        Action act = () => _servicoExternoBase.ValidarResultadoRequisicao(apiResponseEndereco, false);

        // Assert
        act.Should().Throw<Exception>().Where(e => e.Message.EndsWith("Resposta invalida!"));
    }


    [Fact]
    public void Deve_Validar_Uma_Resposta_Com_Corpo()
    {
        // Arrange
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        var endereco = _fakerEndereco.Generate();
        var apiResponseEndereco = new ApiResponse<Endereco>(response, endereco, new RefitSettings(), null);

        // Action
        Action act = () => _servicoExternoBase.ValidarResultadoRequisicao(apiResponseEndereco, true);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Deve_Gerar_Excecao_Ao_Validar_Uma_Resposta_Sem_Corpo_Mas_Validando_Corpo()
    {
        // Arrange
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        var apiResponseEndereco = new ApiResponse<Endereco>(response, null, new RefitSettings(), null);

        // Action
        Action act = () => _servicoExternoBase.ValidarResultadoRequisicao(apiResponseEndereco, true);

        // Assert
        act.Should().Throw<Exception>().Where(e => e.Message.EndsWith("Corpo da resposta esta nulo!"));
    }

    [Fact]
    public void Deve_Gerar_Excecao_Ao_Validar_Uma_Resposta_Nula()
    {
        // Arrange

        // Action
        Action act = () => _servicoExternoBase.ValidarResultadoRequisicao<Endereco>(null, false);

        // Assert
        act.Should().Throw<ArgumentNullException>().Where(e => e.Message.Equals("Resposta esta nula! (Parameter 'apiResponse')"));
    }

    [Fact]
    public async Task Deve_Gerar_Excecao_Ao_Validar_Uma_Resposta_Com_Erro_Com_Corpo()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/teste");
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        response.Content = new StringContent("Conteudo da resposta");
        var apiException = await ApiException.Create(request, HttpMethod.Get, response, new RefitSettings(), null);
        var apiResponseEndereco = new ApiResponse<Endereco>(response, null, new RefitSettings(), apiException);

        // Action
        Action act = () => _servicoExternoBase.ValidarResultadoRequisicao(apiResponseEndereco, false);

        // Assert
        act.Should().Throw<Exception>().Where(e => e.Message.Equals("Conteudo da resposta Resposta invalida!"));
    }

    [Fact]
    public async Task Deve_Gerar_Excecao_Ao_Validar_Uma_Resposta_Com_Erro_Sem_Corpo()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/teste");
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        response.Content = null;
        var apiException = await ApiException.Create(request, HttpMethod.Get, response, new RefitSettings(), null);
        var apiResponseEndereco = new ApiResponse<Endereco>(response, null, new RefitSettings(), apiException);

        // Action
        Action act = () => _servicoExternoBase.ValidarResultadoRequisicao(apiResponseEndereco, false);

        // Assert
        act.Should().Throw<Exception>().Where(e => e.Message.Equals("Forbidden Resposta invalida!"));
    }
}