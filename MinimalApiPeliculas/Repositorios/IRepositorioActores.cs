using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;

namespace MinimalApiPelicula.Repositorios
{
    public interface IRepositorioActores
    {
        Task Actualizar(Actor actor);
        Task<bool> Existe(int id);
        Task Eliminar(int id);
        Task<int> Crear(Actor actor);
        Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacion);
        Task<Actor?> ObtenerPorId(int id);
        Task<List<Actor>> ObtenerPorNombre(string nombre);
        Task<List<int>> Existen(List<int> ids);
    }
}