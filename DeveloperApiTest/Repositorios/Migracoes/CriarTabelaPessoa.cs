using FluentMigrator;
using System.Diagnostics.CodeAnalysis;

namespace DeveloperApiTest.Repositorios.Migracoes;

[ExcludeFromCodeCoverage]
[Migration(202207261000)]
public class CriarTabelaPessoa : Migration
{
    public override void Up()
    {
        Create.Table("Pessoa")
            .WithColumn("Id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("Nome").AsString(200).NotNullable()
            .WithColumn("DataNascimento").AsDate().NotNullable()
            .WithColumn("Email").AsString(200).Nullable()
            .WithColumn("Ativo").AsBoolean().NotNullable().WithDefaultValue(true);
    }

    public override void Down()
    {
        Delete.Table("Pessoa");
    }
}
