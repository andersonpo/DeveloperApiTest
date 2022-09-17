using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Interfaces;

namespace DeveloperApiTest.Servicos;

public class ServicoPessoa : ServicoBase<Guid, Pessoa, PessoaDTO>, IServicoPessoa
{
    public ServicoPessoa(IRepositorioPessoa repositorioPessoa, IMapper mapper) : base(repositorioPessoa, mapper)
    {
    }
}
