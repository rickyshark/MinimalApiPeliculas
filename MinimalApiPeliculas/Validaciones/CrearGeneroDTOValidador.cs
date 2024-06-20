using FluentValidation;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Repositorios;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearGeneroDTOValidador:AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador(IRepositorioGeneros repositorioGeneros, IHttpContextAccessor httpContextAccessor)
        {
            var valorDeRutaId = httpContextAccessor.HttpContext?.Request.RouteValues["id"];

            var id = 0;

            if (valorDeRutaId is string valorString)
            {
                int.TryParse(valorString, out id );
            }

            RuleFor(x => x.Nombre).NotEmpty().WithMessage(Utilidades.CampoMensajeObligatorio)
                                    .MaximumLength(10).WithMessage(Utilidades.CampoMaximoLengthObligatorio)
                                    .Must(Utilidades.PrimeraLetraEnMayuscula).WithMessage(Utilidades.PrimeraLetraMayuscula)
                                    .MustAsync(async (nombre, _) =>
                                    {
                                        var existe = await repositorioGeneros.Existe(id, nombre);
                                        return !existe;

                                    }).WithMessage(y => $"Ya existe un genero con este nombre: {y.Nombre}");

        }


    }
}
