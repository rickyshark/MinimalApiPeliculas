namespace MinimalApiPelicula.DTOs
{
    public class CrearPeliculaDTO
    {
        public string Titulo { get; set; } = null!;
        public bool EnCine { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public IFormFile? Poster { get; set; }
    }
}
