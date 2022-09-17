using DeveloperApiTest.Dominio.DTOs;
using FluentValidation;

namespace DeveloperApiTest.Dominio.Validadores;

public class ValidadorPessoa : AbstractValidator<PessoaDTO>
{
    public ValidadorPessoa()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .MinimumLength(3).WithMessage("{PropertyName} tem que ser maior que 3 caracteres.");

        var dataAtual = DateTime.Now;
        RuleFor(p => p.DataNascimento)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .GreaterThan(new DateTime(1900, 01, 01)).WithMessage("{PropertyName} não pode ser menor que 01/01/1900")
            .LessThan(new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day)).WithMessage("{PropertyName} não pode ser maior que hoje");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .MinimumLength(5).WithMessage("{PropertyName} tem que ser maior que 5 caracteres")
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("{PropertyName} é inválido.");

        RuleFor(p => p.Ativo)
            .NotNull().WithMessage("{PropertyName} é obrigatório.");
    }
}
