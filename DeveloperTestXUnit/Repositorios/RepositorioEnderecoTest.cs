using AutoMapper;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Repositorios;

namespace DeveloperTestXUnit.Repositorios;

public class RepositorioEnderecoTest
{

    private readonly IRepositorioEndereco _repositorioEndereco;
    private readonly Mock<IUnidadeDeTrabalho> _unidadeDeTrabalho;
    private readonly IMapper _mapper;

    public RepositorioEnderecoTest(IMapper mapper)
    {
        _mapper = mapper;
        _unidadeDeTrabalho = new Mock<IUnidadeDeTrabalho>();
        _repositorioEndereco = new RepositorioEndereco(_unidadeDeTrabalho.Object);
    }

    [Fact]
    public void Deve_Construtor_Vazio()
    {
        // Arrange

        // Action

        // Assert
        _repositorioEndereco.Should().NotBeNull();
    }
}
