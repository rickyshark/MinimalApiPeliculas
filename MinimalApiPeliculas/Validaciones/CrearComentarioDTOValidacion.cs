using FluentValidation;
using MinimalApiPelicula.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearComentarioDTOValidacion : AbstractValidator<CrearComentarioDTO>
    {
        public CrearComentarioDTOValidacion()
        {
            RuleFor(x => x.Cuerpo).NotEmpty().WithMessage(Utilidades.CampoMensajeObligatorio);
        }
    }
}
