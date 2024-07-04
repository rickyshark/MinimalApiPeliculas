using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPelicula.Repositorios;
using MinimalApiPeliculas.Filtros;
using MinimalApiPeliculas.Servicios;

namespace MinimalApiPelicula.EndPoints
{
    public static class ComentariosEndPoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(x => x
                .Expire(TimeSpan.FromMinutes(1))
                .Tag("comentarios-get")
                .SetVaryByRouteValue(new string[] { "peliculaId" }));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery().AddEndpointFilter<FiltrosValidaciones<CrearComentarioDTO>>().RequireAuthorization();
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltrosValidaciones<CrearComentarioDTO>>().RequireAuthorization(); ;
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization(); ;
            return group;
        }


        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId, IRepositorioComentarios repositorioComentarios,IMapper mapper,
            IRepositorioPeliculas repositorioPeliculas)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);    

            return TypedResults.Ok(comentariosDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, int comentarioId, IRepositorioComentarios repositorioComentarios, IMapper mapper)
        {
            var comentario = await repositorioComentarios.ObtenerPorId(comentarioId);

            if (comentario is null)
            {
                return TypedResults.NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>, NotFound, BadRequest<string>>> Crear(int peliculaId, IRepositorioComentarios repositorioComentarios,IMapper mapper,
            IRepositorioPeliculas repositorioPeliculas, IOutputCacheStore cacheStore, CrearComentarioDTO crearComentario, IServicioUsuarios servicioUsuarios)
        {
            if (! await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentario);
            comentario.PeliculaId = peliculaId;

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            comentario.UsuarioId = usuario.Id;

            var id = await repositorioComentarios.Crear(comentario);

            await cacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Created($"/comentarios/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>, ForbidHttpResult>> Actualizar(int peliculaId, int id,CrearComentarioDTO crearComentarioDTO,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IOutputCacheStore cacheStore, IServicioUsuarios servicioUsuarios)
        {

            if (! await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarioDb = await repositorioComentarios.ObtenerPorId(id);
            if (comentarioDb is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            if (usuario.Id != comentarioDb.UsuarioId)
            {
                //Aunque el usuario este autenticado no tiene permiso para realizar esta accion
                return TypedResults.Forbid();
            }

            comentarioDb.Cuerpo = crearComentarioDTO.Cuerpo;

            await repositorioComentarios.Actualizar(comentarioDb);
            await cacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>, ForbidHttpResult>> Borrar(int peliculaId, int id, IRepositorioComentarios repositorio, IOutputCacheStore cacheStore,
            IServicioUsuarios servicioUsuarios)
        {
            if (! await repositorio.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var comentarioDb = await repositorio.ObtenerPorId(id);
            if (comentarioDb is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            if (usuario.Id != comentarioDb.UsuarioId)
            {
                //Aunque el usuario este autenticado no tiene permiso para realizar esta accion
                return TypedResults.Forbid();
            }

            await repositorio.Eliminar(id);
            await cacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}
