using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPelicula.Repositorios;
using MinimalApiPelicula.Servicios;
using MinimalApiPeliculas.Filtros;

namespace MinimalApiPelicula.EndPoints
{
    public static class ActoresEndPoints
    {
        private static readonly string contenedor = "actores";
        //Grupo de endpoints
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos);//.CacheOutput(x => x.Expire(TimeSpan.FromMinutes(1)).Tag("actores-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapGet("ObtenerPorNombre/{nombreActor}", ObtenerPorNombre);
            group.MapPost("/", Crear).DisableAntiforgery().AddEndpointFilter<FiltrosValidaciones<CrearActorDTO>>();
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltrosValidaciones<CrearActorDTO>>();
            group.MapDelete("/{id:int}", Borrar);
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerTodos(IRepositorioActores repositorio, IMapper mapper, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };

            var actores = await repositorio.ObtenerTodos(paginacion);
            var actoresDTOs = mapper.Map<List<ActorDTO>>(actores);  

            return TypedResults.Ok(actoresDTOs);
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerPorNombre(string nombreActor,IRepositorioActores repositorio, IMapper mapper)
        {
            var actores = await repositorio.ObtenerPorNombre(nombre : nombreActor);
            var actoresDTOs = mapper.Map<List<ActorDTO>>(actores);

            return TypedResults.Ok(actoresDTOs);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerPorId(int id, IRepositorioActores repositorio, IMapper mapper)
        {
            var actor = await repositorio.ObtenerPorId(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            var actorDTO = mapper.Map<ActorDTO>(actor);

            return TypedResults.Ok(actorDTO);   
        }

        static async Task<Results<Created<ActorDTO>, ValidationProblem>> Crear([FromForm] CrearActorDTO crearActor, IRepositorioActores repositorio, 
            IOutputCacheStore cacheStore, IMapper mapper,
            IAlmacenadorArchivo almacenadorArchivo)
        {

            var actor = mapper.Map<Actor>(crearActor);

            if (crearActor.Foto is not null)
            {
                var url = await almacenadorArchivo.Almacenar(contenedor, crearActor.Foto);
                actor.Foto = url;
            }

            var id = await repositorio.Crear(actor);
            await cacheStore.EvictByTagAsync("actores-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);

            return TypedResults.Created($"/actores/{id}", actorDTO);
        }

        static async Task<Results<NotFound, NoContent>> Actualizar(int id, [FromForm] CrearActorDTO crearActor, IRepositorioActores repositorio, IAlmacenadorArchivo almacenadorArchivo,
            IOutputCacheStore cacheStore, IMapper mapper)
        {
            var actorDB = await repositorio.ObtenerPorId(id);
            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            var actorActualizar = mapper.Map<Actor>(crearActor);
            actorActualizar.Id = id;
            actorActualizar.Foto = actorDB.Foto;
            if (crearActor.Foto is not null)
            {
                var url = await almacenadorArchivo.Editar(actorActualizar.Foto, contenedor, crearActor.Foto);
                actorActualizar.Foto = url;
            }

            await repositorio.Actualizar(actorActualizar);
            await cacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound,NoContent>> Borrar(int id, IRepositorioActores repositorio,IAlmacenadorArchivo archivo, IOutputCacheStore cacheStore)
        {
            var actor = await repositorio.ObtenerPorId(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Eliminar(id);
            await archivo.Borrar(actor.Foto, contenedor);
            await cacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }
    }
}
