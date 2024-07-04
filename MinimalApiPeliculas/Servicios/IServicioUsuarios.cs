using Microsoft.AspNetCore.Identity;

namespace MinimalApiPeliculas.Servicios
{
    public interface IServicioUsuarios
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}