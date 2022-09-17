namespace DeveloperApiTest.Dominio.DTOs;
public class EnderecoDTO : DTOBase<Guid>
{
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
}
