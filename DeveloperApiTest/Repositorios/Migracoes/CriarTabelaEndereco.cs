using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Repositorios.Migracoes;

[ExcludeFromCodeCoverage]
[Migration(202207301300)]
public class CriarTabelaEndereco : Migration
{
    public override void Down()
    {
        Delete.Index("Endereco_Cep_Asc");
        Delete.Index("Endereco_Cep_Unique");
        Delete.Table("Endereco");
    }

    public override void Up()
    {
        Create.Table("Endereco")
            .WithColumn("Id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("Logradouro").AsString(250).Nullable()
            .WithColumn("Complemento").AsString(250).Nullable()
            .WithColumn("Bairro").AsString(250).Nullable()
            .WithColumn("Cidade").AsString(250).NotNullable()
            .WithColumn("UF").AsString(2).Nullable()
            .WithColumn("Cep").AsString(8).NotNullable();

        Create.Index("Endereco_cep_unique")
            .OnTable("Endereco")
            .OnColumn("cep").Unique();

        Create.Index("Endereco_cep_asc")
            .OnTable("Endereco")
            .OnColumn("cep").Ascending();
    }
}
