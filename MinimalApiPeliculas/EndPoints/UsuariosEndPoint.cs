using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Filtros;
using MinimalApiPeliculas.Utilidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalApiPeliculas.EndPoints
{
    public static class UsuariosEndPoint
    {
        public static RouteGroupBuilder MapUsuarios(this RouteGroupBuilder group)
        {
            group.MapPost("/registrar", Registrar).AddEndpointFilter<FiltrosValidaciones<CredencialesUsurioDTO>>();
            return group;
        }

        static async Task<Results<Ok<RespuestaAutenticacionDTO>, BadRequest<IEnumerable<IdentityError>>>> Registrar(CredencialesUsurioDTO usurioDTO, 
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var usuario = new IdentityUser
            {
                UserName = usurioDTO.Email,
                Email = usurioDTO.Email,
            };

            var resultado = await userManager.CreateAsync(usuario);

            if (resultado.Succeeded)
            {
                var credencialesRespuesta = ConstruirToken(credencialesUsurio : usurioDTO, configuration: configuration);
                return TypedResults.Ok(credencialesRespuesta);
            }
            else
            {
                return TypedResults.BadRequest(resultado.Errors);
            }

        }

        //Metodo para construir nuestro token
        private static RespuestaAutenticacionDTO ConstruirToken(CredencialesUsurioDTO credencialesUsurio, IConfiguration configuration)
        {
            //Esta es la seccion de data de mi token 
            //JWT se divide en tres secciones 1.  cabezara, 2. data relacionada al usuario, 3. firma o certificado
            //No colocar data sensible. ej card numer
            var claims = new List<Claim>
            {
                new Claim("email", credencialesUsurio.Email),
                new Claim("otro valor", "puedo colocar lo que quiera")
            };

            var llave = Llaves.ObtenerLlave(configuration);
            //Punto 3 JWT. Firma del token
            var creds = new SigningCredentials(llave.First(), SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            var tokenDeSeguridad = new JwtSecurityToken(issuer:null,audience:null,claims : claims,expires :expiracion,signingCredentials: creds);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                TiempoExpirecion = expiracion,
            };
        }
    }
}
