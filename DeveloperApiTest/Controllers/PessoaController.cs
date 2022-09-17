using DeveloperApiTest.Dominio.DTOs;
using DeveloperApiTest.Dominio.Validadores;
using DeveloperApiTest.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperApiTest.Controllers;

public class PessoaController : BaseController
{
    private readonly IServicoPessoa _pessoaServico;

    public PessoaController(IServicoPessoa pessoaServico)
    {
        _pessoaServico = pessoaServico;
    }

    [ApiVersion("1.0")]
    [HttpGet]
    public async Task<IActionResult> ListarPessoasAsync()
    {
        var resultado = await _pessoaServico.Listar();
        return Ok(resultado);
    }

    [ApiVersion("1.0")]
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPessoaAsync([FromRoute] Guid id)
    {
        var erro = ValidarId(id);
        if (erro != null)
        {
            return BadRequest(erro);
        }

        var resultado = await _pessoaServico.ObterAsync(id);
        if (resultado != null)
            return Ok(resultado);
        else
            return NotFound();
    }

    [ApiVersion("1.0")]
    [HttpPost]
    public async Task<IActionResult> CriarPessoaAsync([FromBody] PessoaDTO pessoaDTO)
    {
        var erros = await ValidarDTO(new ValidadorPessoa(), pessoaDTO);
        if (erros != null)
        {
            return BadRequest(erros);
        }

        var resultado = await _pessoaServico.CriarAsync(pessoaDTO);
        if (resultado != null)
            return Created($"/{resultado.Id}", resultado);
        else
            return NotFound();
    }

    [ApiVersion("1.0")]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarPessoaAsync([FromRoute] Guid id, [FromBody] PessoaDTO pessoaDTO)
    {
        var erro = ValidarId(id);
        if (erro != null)
        {
            return BadRequest(erro);
        }

        var erros = await ValidarDTO(new ValidadorPessoa(), pessoaDTO);
        if (erros != null)
        {
            return BadRequest(erros);
        }

        var resultado = _pessoaServico.Atualizar(id, pessoaDTO);
        if (resultado != null)
            return Ok(resultado);
        else
            return NotFound();
    }

    [ApiVersion("1.0")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirPessoaAsync([FromRoute] Guid id)
    {
        var erro = ValidarId(id);
        if (erro != null)
        {
            return BadRequest(erro);
        }

        var resultado = await _pessoaServico.ExcluirAsync(id);
        if (resultado)
            return Ok();
        else
            return NotFound();
    }
}
