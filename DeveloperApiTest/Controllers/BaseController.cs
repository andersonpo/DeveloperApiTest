using DeveloperApiTest.Dominio.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperApiTest.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{
    public ErroDTO? ValidarId<TId>(TId id) where TId : IComparable, IEquatable<TId>
    {
        var listaDeErros = new List<string>();
        if (id.GetType() == typeof(Guid))
        {
            if (Guid.Parse(id.ToString()!) == Guid.Empty)
            {
                listaDeErros.Add("Id não pode ser zero.");
                return new ErroDTO(listaDeErros);
            }
        }

        if (id.GetType() == typeof(int) || id.GetType() == typeof(long))
        {
            if (Convert.ToInt64(id) <= 0)
            {
                listaDeErros.Add("Id tem que ser maior que zero.");
                return new ErroDTO(listaDeErros);
            }
        }

        return null;
    }

    public async Task<ErroDTO?> ValidarDTO<TValidador, TEntidadeDTO>(TValidador validador, TEntidadeDTO entidadeDTO) where TValidador : AbstractValidator<TEntidadeDTO>
    {
        if (validador == null) return null;

        var valido = await validador.ValidateAsync(entidadeDTO);

        if (!valido.IsValid)
        {
            var listaDeErros = new List<string>();
            valido.Errors.ForEach(e => listaDeErros.Add(e.ErrorMessage));
            return new ErroDTO(listaDeErros);
        }

        return null;
    }
}
