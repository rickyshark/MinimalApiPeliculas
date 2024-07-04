using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class EditarClaimDTOValidador :AbstractValidator<EditarClaimDTO>
    {
        public EditarClaimDTOValidador()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoMensajeObligatorio)
                .MaximumLength(255).WithMessage(Utilidades.CampoMaximoLengthObligatorio)
                .EmailAddress().WithMessage(Utilidades.MensajeEmail);
        }
    }
}
