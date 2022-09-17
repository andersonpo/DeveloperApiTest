namespace DeveloperApiTest.Dominio.DTOs;

public class DTOBase<TId>
    where TId : IComparable, IEquatable<TId>
{
    public TId? Id { get; set; }
}
