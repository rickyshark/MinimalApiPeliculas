
using AutoMapper;
using MinimalApiPelicula.Repositorios;

namespace MinimalApiPeliculas.Filtros
{
    public class FiltroDePrueba : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext contexto, EndpointFilterDelegate next)
        {

            //Esta forma no esta mal, pero si en un momento cambia el orden de los parametros, este codigo no funcionara.
            //var param1 = (int)contexto.Arguments[0]!;
            //var param2 = (IRepositorioGeneros)contexto.Arguments[1]!;
            //var param3 = (IMapper)contexto.Arguments[2]!;

            //Otra forma de obtener el valor de los parametros (Utilizando LINQ).
            var IdGenero = contexto.Arguments.OfType<int>().FirstOrDefault();
            var RepositorioGenero = contexto.Arguments.OfType<IRepositorioGeneros>().FirstOrDefault();
            var Mapper = contexto.Arguments.OfType<IMapper>().FirstOrDefault();

            var resultado = await next(contexto);

            //Este codigo se ejecuta despiues del endpoint.

            return resultado;
        }
    }
}
