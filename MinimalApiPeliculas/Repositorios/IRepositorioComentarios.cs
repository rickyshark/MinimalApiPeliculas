using MinimalApiPelicula.Entidades;

namespace MinimalApiPelicula.Repositorios
{
    public interface IRepositorioComentarios
    {
        Task Actualizar(Comentario comentario);
        Task<int> Crear(Comentario comentario);
        Task Eliminar(int id);
        Task<bool> Existe(int id);
        Task<Comentario?> ObtenerPorId(int id);
        Task<List<Comentario>> ObtenerTodos(int peliculaId);
    }
}