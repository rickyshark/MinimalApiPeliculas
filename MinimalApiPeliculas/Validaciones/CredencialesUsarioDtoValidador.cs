using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CredencialesUsarioDtoValidador :AbstractValidator<CredencialesUsurioDTO>
    {
        public CredencialesUsarioDtoValidador()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoMensajeObligatorio)
                                .MaximumLength(256).WithMessage(Utilidades.CampoMaximoLengthObligatorio)
                                .EmailAddress().WithMessage(Utilidades.MensajeEmail);

            RuleFor(x => x.Password).NotEmpty().WithMessage(Utilidades.CampoMensajeObligatorio);
        }
    }
}
