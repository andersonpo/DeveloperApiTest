using DeveloperApiTest.Infraestrutura.Extensoes;
using DeveloperApiTest.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperApiTest.Controllers;

public class CepController : BaseController
{
    private readonly IServicoCep _cepService;

    public CepController(IServicoCep cepService)
    {
        _cepService = cepService;
    }

    [ApiVersion("1.0")]
    [HttpGet("{cep}")]
    public async Task<IActionResult> ObterEnderecoPeloCep([FromRoute] string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
        {
            return BadRequest(new { message = "Cep é obrigatório." });
        }

        var cepNumerico = cep.ApenasNumeros();
        if (cepNumerico == 0)
        {
            return BadRequest(new { message = "Cep apenas número." });
        }

        var resultado = await _cepService.ObterEnderecoPeloCep(cepNumerico.ToString());

        return Ok(resultado);
    }

    [ApiVersion("1.0")]
    [HttpGet("{uf}/{cidade}/{logradouro}")]
    public async Task<IActionResult> ObterCepPeloEndereco([FromRoute] string uf, [FromRoute] string cidade, [FromRoute] string logradouro)
    {
        var resultado = await _cepService.ObterCepPeloEndereco(uf, cidade, logradouro);

        return Ok(resultado);
    }

    [ApiVersion("1.1")]
    [HttpGet("teste")]
    public IActionResult RotaTesteSwagger()
    {
        return Ok(new { message = "Sucesso" });
    }
}
