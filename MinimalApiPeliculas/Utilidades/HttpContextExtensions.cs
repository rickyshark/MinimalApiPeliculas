using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace MinimalApiPelicula.Utilidades
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext HttpContext, IQueryable<T> queryable)
        {
            if (HttpContext is null)
            {
                throw new ArgumentNullException(nameof(HttpContext));
            }
            double cantidad = await queryable.CountAsync();
            HttpContext.Response.Headers.Append("cantidadTotalRegistros", cantidad.ToString());
        }
    }
}
