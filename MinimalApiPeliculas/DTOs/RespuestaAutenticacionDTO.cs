namespace MinimalApiPeliculas.DTOs
{
    public class RespuestaAutenticacionDTO
    {
        public string Token { get; set; } = null!;
        public DateTime TiempoExpirecion { get; set; }
    }
}
