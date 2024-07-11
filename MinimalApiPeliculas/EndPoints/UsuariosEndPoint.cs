using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Filtros;
using MinimalApiPeliculas.Servicios;
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
            group.MapPost("/login", Login).AddEndpointFilter<FiltrosValidaciones<CredencialesUsurioDTO>>();
            group.MapPost("/haceradmin", HacerAdmin).AddEndpointFilter<FiltrosValidaciones<EditarClaimDTO>>()
                .RequireAuthorization("esadmin");
            group.MapPost("/removeradmin", RemoverAdmin).AddEndpointFilter<FiltrosValidaciones<EditarClaimDTO>>()
                .RequireAuthorization("esadmin");

            group.MapGet("/renovartoken", RenovarToken).RequireAuthorization();
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

            var resultado = await userManager.CreateAsync(usuario, usurioDTO.Password);

            if (resultado.Succeeded)
            {
                var credencialesRespuesta = await ConstruirToken(credencialesUsurio : usurioDTO, configuration: configuration, userManager);
                return TypedResults.Ok(credencialesRespuesta);
            }
            else
            {
                return TypedResults.BadRequest(resultado.Errors);
            }

        }

        static async Task<Results<Ok<RespuestaAutenticacionDTO>, BadRequest<string>>> Login(CredencialesUsurioDTO credencialesUsurioDTO,
            [FromServices] SignInManager<IdentityUser> signInManager, [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {

            var usuario = await userManager.FindByEmailAsync(credencialesUsurioDTO.Email);
            if (usuario is null)
            {
                return TypedResults.BadRequest("Login incorrecto A11");
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario,credencialesUsurioDTO.Password,lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await ConstruirToken(credencialesUsurioDTO, configuration, userManager);
                return TypedResults.Ok(respuestaAutenticacion);
            }
            else
            {
                return TypedResults.BadRequest("Login incorrecto A12");
            }

        }

        static async Task<Results<NoContent, NotFound<string>, Ok<string>>> HacerAdmin(EditarClaimDTO editarClaimDTO, [FromServices] UserManager<IdentityUser> userManager)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario is null)
            {
                return TypedResults.NotFound("Usuario no encontrado");
            }

            var result = await userManager.AddClaimAsync(usuario, new Claim("esadmin", "true"));
            if (result.Succeeded)
            {
                return TypedResults.Ok("Claim agregado con exitosamente");
            }
            else
            {
                return TypedResults.NoContent();
            }
        }

        static async Task<Results<NoContent, NotFound<string>>> RemoverAdmin(EditarClaimDTO editarClaimDTO, [FromServices] UserManager<IdentityUser> userManager)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);

            if (usuario is null)
            {
                return TypedResults.NotFound("Usuario no encontrado");
            }

            await userManager.RemoveClaimAsync(usuario, new Claim("esadmin", "true"));
            return TypedResults.NoContent();
        }

        public async static Task<Results<Ok<RespuestaAutenticacionDTO>, NotFound>> RenovarToken(IServicioUsuarios servicioUsuarios,IConfiguration configuration,
            [FromServices] UserManager<IdentityUser> userManager)
        {
            var usuasio = await servicioUsuarios.ObtenerUsuario();
            if (usuasio is null)
            {
                return TypedResults.NotFound();
            }

            var credencialesUsuarioDTO =  new CredencialesUsurioDTO { Email = usuasio.Email!};

            var respuestaAutenticacion = await ConstruirToken(credencialesUsuarioDTO, configuration, userManager);

            return TypedResults.Ok(respuestaAutenticacion);
        }

        //Metodo para construir nuestro token
        private async static Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsurioDTO credencialesUsurio, IConfiguration configuration,
            UserManager<IdentityUser> userManager)
        {
            //Esta es la seccion de data de mi token 
            //JWT se divide en tres secciones 1.  cabezara, 2. data relacionada al usuario, 3. firma o certificado
            //No colocar data sensible. ej card numer
            var claims = new List<Claim>
            {
                new Claim("email", credencialesUsurio.Email),
                new Claim("otro valor", "puedo colocar lo que quiera")
            };

            var usuario = await userManager.FindByNameAsync(credencialesUsurio.Email);
            var claimDb = await userManager.GetClaimsAsync(usuario!);

            claims.AddRange(claimDb);


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
