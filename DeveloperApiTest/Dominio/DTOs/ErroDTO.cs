namespace DeveloperApiTest.Dominio.DTOs;

public class ErroDTO
{
    public ErroDTO(List<string> erros)
    {
        Erros = erros.ToList();
    }
    public List<string> Erros { get; private set; }
}
