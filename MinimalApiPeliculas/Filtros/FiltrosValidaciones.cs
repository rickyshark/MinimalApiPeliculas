
using FluentValidation;
using MinimalApiPelicula.DTOs;

namespace MinimalApiPeliculas.Filtros
{
    public class FiltrosValidaciones<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validador = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validador == null)
            {
                return await next(context);
            }

            var insumoAValidar = context.Arguments.OfType<T>().FirstOrDefault();

            if (insumoAValidar is null)
            {
                return TypedResults.Problem("No fue encontrada la entidad a validar");
            }

            var resultadoValidacion = await validador.ValidateAsync(insumoAValidar);

            if (!resultadoValidacion.IsValid)
            {
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());
            }

            return await next(context);
        }
    }
}
