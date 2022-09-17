using DeveloperApiTest.Dominio.DTOs;
using FluentValidation;

namespace DeveloperApiTest.Dominio.Validadores;

public class ValidadorEndereco : AbstractValidator<EnderecoDTO>
{
    public ValidadorEndereco()
    {
        RuleFor(e => e.Cep)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .Length(8).WithMessage("{PropertyName} tem que ter 8 digitos.");

        RuleFor(e => e.UF)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .Length(2).WithMessage("{PropertyName} tem que ter 2 caracteres.");

        RuleFor(e => e.Cidade)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .MinimumLength(3).WithMessage("{PropertyName}  tem que ser maior que 3 caracteres.");


        RuleFor(e => e.Bairro)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .MinimumLength(3).WithMessage("{PropertyName}  tem que ser maior que 3 caracteres.");

        RuleFor(e => e.Logradouro)
            .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
            .MinimumLength(3).WithMessage("{PropertyName}  tem que ser maior que 3 caracteres.");
    }
}
