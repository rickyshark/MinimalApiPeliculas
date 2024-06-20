using FluentValidation;
using MinimalApiPelicula.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearPeliculaDTOValidacion :AbstractValidator<CrearPeliculaDTO>
    {
        public CrearPeliculaDTOValidacion()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage(Utilidades.CampoMensajeObligatorio);
        }
    }
}
