using DeveloperApiTest.Dominio.Entidade;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Repositorios;

[ExcludeFromCodeCoverage]
public class BancoDeDadosContexto : DbContext
{
    public BancoDeDadosContexto(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Pessoa>? Pessoa { get; set; }
    public DbSet<Endereco>? Endereco { get; set; }

}
