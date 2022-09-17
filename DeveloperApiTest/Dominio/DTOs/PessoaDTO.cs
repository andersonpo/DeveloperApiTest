namespace DeveloperApiTest.Dominio.DTOs;

public class PessoaDTO : DTOBase<Guid>
{
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; } = DateTime.MinValue;
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}
