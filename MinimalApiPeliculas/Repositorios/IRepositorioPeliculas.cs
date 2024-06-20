using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPelicula.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task AsignarActores(int id, List<ActorPelicula> actores);
        Task AsignarGeneros(int id, List<int> generosId);
        Task<int> Crear(Pelicula pelicula);
        Task Eliminar(int id);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}