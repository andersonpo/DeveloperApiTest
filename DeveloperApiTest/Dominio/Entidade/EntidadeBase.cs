namespace DeveloperApiTest.Dominio.Entidade;

public class EntidadeBase<TId>
    where TId : IComparable, IEquatable<TId>
{
    public TId? Id { get; set; }
}
