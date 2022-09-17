using AutoMapper;
using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Entidade;
using DeveloperApiTest.Dominio.Externo;

namespace DeveloperApiTest.Dominio.Mapeamentos;

public class Mapeamentos : Profile
{
    public Mapeamentos()
    {
        CreateMap<PessoaDTO, Pessoa>().ReverseMap();
        CreateMap<EnderecoDTO, Endereco>().ReverseMap();
        CreateMap<EnderecoDTO, ViaCepEndereco>()
            .ForMember(origem => origem.Localidade, cfg => cfg.MapFrom(destino => destino.Cidade))
            .ReverseMap();
    }
}
