
namespace MinimalApiPelicula.Entidades
{
    public class Genero
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = new List<GeneroPelicula>();

    }
}
