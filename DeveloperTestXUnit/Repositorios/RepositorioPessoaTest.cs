using AutoMapper;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;
using DeveloperApiTest.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using Moq.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperTestXUnit.Repositorios;

public class RepositorioPessoaTest
{

    private readonly IRepositorioPessoa _repositorioPessoa;
    private readonly Mock<IUnidadeDeTrabalho> _unidadeDeTrabalho;
    private readonly Mock<DbSet<Pessoa>> _DbSetPessoa;
    private readonly Faker<Pessoa> _fakerPessoa;
    private readonly IMapper _mapper;

    public RepositorioPessoaTest(IMapper mapper)
    {
        _mapper = mapper;
        _unidadeDeTrabalho = new Mock<IUnidadeDeTrabalho>();
        _DbSetPessoa = new Mock<DbSet<Pessoa>>();
        _repositorioPessoa = new RepositorioPessoa(_unidadeDeTrabalho.Object);
        _fakerPessoa = new Faker<Pessoa>("pt_BR")
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
        var pessoa = _fakerPessoa.Generate();
        var resultadoEsperado = _mapper.Map<Pessoa>(pessoa);

        var entityEntryPessoa = new EntityEntry<Pessoa>(
                                    new InternalEntityEntry(
                                        new Mock<IStateManager>().Object,
                                        new RuntimeEntityType("Pessoa", typeof(Pessoa), false, new RuntimeModel(), null, null, ChangeTrackingStrategy.Snapshot, null, false),
                                        pessoa
                                    ));

        _unidadeDeTrabalho.Setup(u => u.Registros<Guid, Pessoa>().AddAsync(It.IsAny<Pessoa>(), It.IsAny<CancellationToken>())).ReturnsAsync(entityEntryPessoa);

        // Action
        var resultado = await _repositorioPessoa.CriarAsync(pessoa);

        // Assert
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public void Deve_Atualizar_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var pessoa = _fakerPessoa.Generate();
        var resultadoEsperado = _mapper.Map<Pessoa>(pessoa);

        var entityEntryPessoa = new EntityEntry<Pessoa>(
                                    new InternalEntityEntry(
                                        new Mock<IStateManager>().Object,
                                        new RuntimeEntityType("Pessoa", typeof(Pessoa), false, new RuntimeModel(), null, null, ChangeTrackingStrategy.Snapshot, null, false),
                                        pessoa
                                    ));

        _unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>().Update(It.IsAny<Pessoa>())).Returns(entityEntryPessoa);

        // Action
        var resultado = _repositorioPessoa.Atualizar(pessoa);

        // Assert
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var pessoa = _fakerPessoa.Generate();
        var resultadoEsperado = _mapper.Map<Pessoa>(pessoa);
        var listaPessoa = _fakerPessoa.Generate(5);
        listaPessoa.Add(pessoa);

        _unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>()).ReturnsDbSet(listaPessoa);

        // Action
        var resultado = await _repositorioPessoa.ObterAsync(pessoa.Id);

        // Assert
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_A_Lista_De_Pessoa_Com_Sucesso()
    {
        // Arrange
        var resultadoEsperado = _fakerPessoa.Generate(5);

        //_unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>()).ReturnsDbSet(resultadoEsperado);
        _unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>().AsQueryable()).Returns(resultadoEsperado.AsQueryable());

        // Action
        var resultado = await _repositorioPessoa.ListarAsync();

        // Assert
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_A_Lista_De_Pessoa_Com_Filtro_Com_Sucesso()
    {
        // Arrange
        var listaMock = _fakerPessoa.Generate(5);
        var resultadoEsperado = listaMock.Where(p => p.Ativo);

        //_unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>()).ReturnsDbSet(resultadoEsperado);
        _unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>().AsQueryable()).Returns(listaMock.AsQueryable());

        // Action
        var resultado = await _repositorioPessoa.ListarAsync(filtro: p => p.Ativo);

        // Assert
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task Deve_Obter_A_Lista_De_Pessoa_Com_Ordenacao_Com_Sucesso()
    {
        // Arrange
        var listaMock = _fakerPessoa.Generate(5);
        var resultadoEsperado = listaMock.OrderBy(p => p.Email);

        //_unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>()).ReturnsDbSet(resultadoEsperado);
        _unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>().AsQueryable()).Returns(listaMock.AsQueryable());

        // Action
        var resultado = await _repositorioPessoa.ListarAsync(orderBy: p => p.Email);

        // Assert
        resultado.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact(Skip = "Avaliar como marcar state deleted")]
    public async Task Deve_Excluir_Uma_Pessoa_Com_Sucesso()
    {
        // Arrange
        var pessoa = _fakerPessoa.Generate();
        var listaPessoa = _fakerPessoa.Generate(5);
        listaPessoa.Add(pessoa);

        _unidadeDeTrabalho.Setup(x => x.Registros<Guid, Pessoa>()).ReturnsDbSet(listaPessoa);
        
        var stateManagerMock = new Mock<IStateManager>();

        var internalEntityEntry = new InternalEntityEntry(
            stateManagerMock.Object, 
            new RuntimeEntityType("Pessoa", typeof(Pessoa), false, new RuntimeModel(), null, null, ChangeTrackingStrategy.ChangingAndChangedNotifications, null, false), 
            pessoa);

        var entityEntryPessoa = new EntityEntry<Pessoa>(internalEntityEntry);
        entityEntryPessoa.State = EntityState.Deleted;

        _unidadeDeTrabalho.Setup(u => u.Registros<Guid, Pessoa>().Remove(It.IsAny<Pessoa>())).Returns(entityEntryPessoa);

        // Action
        var resultado = await _repositorioPessoa.ExcluirAsync(pessoa.Id);

        // Assert
        resultado.Should().BeTrue();
    }
}
