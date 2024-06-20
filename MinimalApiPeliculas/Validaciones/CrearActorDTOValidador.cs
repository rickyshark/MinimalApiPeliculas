using FluentValidation;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Repositorios;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearActorDTOValidador : AbstractValidator<CrearActorDTO>
    {
        public CrearActorDTOValidador(IRepositorioActores repositorioActores, IHttpContextAccessor httpContextAccessor)
        {

            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilidades.CampoMensajeObligatorio)
                                .MaximumLength(200).WithMessage(Utilidades.CampoMaximoLengthObligatorio);

            var fechaMinima = new DateTime(1900, 1, 1);

            RuleFor(x => x.FechaNacimiento).GreaterThanOrEqualTo(fechaMinima)
                .WithMessage(Utilidades.GreaterThanOrEqualToMensaje(fechaMinima));
        }

    }
}
