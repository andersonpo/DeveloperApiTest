using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Validadores;
using DeveloperApiTest.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperApiTest.Controllers;

public class EnderecoController : BaseController
{
    private readonly IServicoEndereco _enderecoServico;

    public EnderecoController(IServicoEndereco enderecoServico)
    {
        _enderecoServico = enderecoServico;
    }

    [ApiVersion("1.0")]
    [HttpGet]
    public async Task<IActionResult> ListarEnderecosAsync()
    {
        var resultado = await _enderecoServico.Listar();
        return Ok(resultado);
    }

    [ApiVersion("1.0")]
    [HttpGet("cidade/{uf}/{cidade}")]
    public async Task<IActionResult> ListarEnderecosPorUfECidadeAsync([FromRoute] string uf, [FromRoute] string cidade)
    {
        var resultado = await _enderecoServico.ListarPorUfeCidade(uf, cidade);
        return Ok(resultado);
    }

    [ApiVersion("1.0")]
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterEnderecoAsync([FromRoute] Guid id)
    {
        var erro = ValidarId(id);
        if (erro != null)
        {
            return BadRequest(erro);
        }

        var resultado = await _enderecoServico.ObterAsync(id);
        if (resultado != null)
            return Ok(resultado);
        else
            return NotFound();
    }

    [ApiVersion("1.0")]
    [HttpPost]
    public async Task<IActionResult> CriarEnderecoAsync([FromBody] EnderecoDTO enderecoDTO)
    {
        var erros = await ValidarDTO(new ValidadorEndereco(), enderecoDTO);
        if (erros != null)
        {
            return BadRequest(erros);
        }

        var resultado = await _enderecoServico.CriarAsync(enderecoDTO);
        if (resultado != null)
            return Created($"/{resultado.Id}", resultado);
        else
            return NotFound();
    }

    [ApiVersion("1.0")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarEnderecoAsync([FromRoute] Guid id, [FromBody] EnderecoDTO enderecoDTO)
    {
        var erro = ValidarId(id);
        if (erro != null)
        {
            return BadRequest(erro);
        }

        var erros = await ValidarDTO(new ValidadorEndereco(), enderecoDTO);
        if (erros != null)
        {
            return BadRequest(erros);
        }

        var resultado = _enderecoServico.Atualizar(id, enderecoDTO);
        if (resultado != null)
            return Ok(resultado);
        else
            return NotFound();
    }

    [ApiVersion("1.0")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirEnderecoAsync([FromRoute] Guid id)
    {
        var erro = ValidarId(id);
        if (erro != null)
        {
            return BadRequest(erro);
        }

        var resultado = await _enderecoServico.ExcluirAsync(id);
        if (resultado)
            return Ok();
        else
            return NotFound();
    }
}
