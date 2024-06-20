namespace MinimalApiPelicula.Servicios
{
    public interface IAlmacenadorArchivo
    {
        Task Borrar(string? ruta, string contenedor);
        Task<string> Almacenar(string contenedor, IFormFile archivo);

        async Task<string> Editar(string? ruta, string contendor, IFormFile archivo)
        {
            await Borrar(ruta, contendor);
            return await Almacenar(contendor, archivo);
        }

    }
}
