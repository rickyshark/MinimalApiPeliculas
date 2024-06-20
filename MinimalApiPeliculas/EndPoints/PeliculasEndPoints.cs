using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPelicula.Repositorios;
using MinimalApiPelicula.Servicios;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Filtros;
using System.Runtime.CompilerServices;

namespace MinimalApiPelicula.EndPoints
{
    public static class PeliculasEndPoints
    {
        private static readonly string contenedor = "peliculas";

        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapPost("/", Crear).DisableAntiforgery().AddEndpointFilter<FiltrosValidaciones<CrearPeliculaDTO>>();
            group.MapGet("/", ObtenerTodos).CacheOutput(x => x.Expire(TimeSpan.FromMinutes(1)).Tag("peliculas-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPut("{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltrosValidaciones<CrearPeliculaDTO>>();
            group.MapDelete("{id:int}", Borrar);
            group.MapPost("/{id:int}/asignargeneros", AsignarGeneros);
            group.MapPost("/{id:int}/asignaractores", AsignarActores);

            return group;
        }


        static async Task<Ok<List<PeliculaDTO>>> ObtenerTodos(IRepositorioPeliculas repositorio, IMapper mapper, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };

            var peliculas = await repositorio.ObtenerTodos(paginacion);
            var peliculasDTOs = mapper.Map<List<PeliculaDTO>>(peliculas);

            return TypedResults.Ok(peliculasDTOs);
        }

        static async Task<Results<NotFound, Ok<PeliculaDTO>>> ObtenerPorId(int id, IRepositorioPeliculas repositorio, IMapper mapper)
        {
            var pelicula = await repositorio.ObtenerPorId(id);
            if (pelicula is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDTO);
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio, IAlmacenadorArchivo archivo,
            IMapper mapper, IOutputCacheStore cacheStore)
        {

            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);
            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await archivo.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id  = await repositorio.Crear(pelicula);
            await cacheStore.EvictByTagAsync("peliculas-get", default);

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            
            return TypedResults.Created($"/peliculas/{id}", peliculaDTO);
        }

        static async Task<Results<NotFound, NoContent>> Actualizar(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO,IRepositorioPeliculas repositorio,
           IOutputCacheStore cacheStore, IMapper mapper , IAlmacenadorArchivo archivo)
        {

            var peliculaDb = await repositorio.ObtenerPorId(id);

            if (peliculaDb is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaParaActualizar.Id = id;
            peliculaParaActualizar.Poster = peliculaDb!.Poster;

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await archivo.Editar(peliculaParaActualizar.Poster, contenedor, crearPeliculaDTO.Poster);
                peliculaParaActualizar.Poster = url;
            }

            await repositorio.Actualizar(peliculaParaActualizar);
            await cacheStore.EvictByTagAsync("peliculas-get", default);


            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Borrar(int id, IRepositorioPeliculas repositorio, IAlmacenadorArchivo archivo,
            IOutputCacheStore cacheStore)
        {
            var peliculaDb = await repositorio.ObtenerPorId(id);
            if (peliculaDb is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Eliminar(id);
            await archivo.Borrar(peliculaDb.Poster, contenedor);
            await cacheStore.EvictByTagAsync("peliculas-get", default);

            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds, IRepositorioPeliculas repositorioPeliculas,
            IRepositorioGeneros repositorioGeneros)
        {
            if (! await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var generosExistentes = new List<int>();

            if (generosIds.Count != 0)
            {
                generosExistentes = await repositorioGeneros.Existen(generosIds);
            }

            if (generosExistentes.Count != generosIds.Count)
            {
                //Traeme los generosId que no se encuentren en generosExistentes
                var generosNoExistentes = generosIds.Except(generosExistentes);
                return TypedResults.BadRequest($"El/los genero/s con id {string.Join(",", generosNoExistentes)} no existen");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosIds);

            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarActores(int id, List<AsignarActorPeliculaDTO> actorDTO,
            IRepositorioPeliculas repositorioPeliculas, IRepositorioActores repositorioActores, IMapper mapper)
        {

            if (! await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var actoresExistente = new List<int>(); 
            var actoresIds = actorDTO.Select(a => a.ActorId).ToList();
            if (actorDTO.Count >= 1)
            {
                actoresExistente = await repositorioActores.Existen(actoresIds);
            }

            if (actoresExistente.Count != actorDTO.Count)
            {
                var actoresNoExistente = actoresIds.Except(actoresExistente);
                return TypedResults.BadRequest($"Los actores con id {string.Join(",", actoresNoExistente)} no existen");
            }

            var actorPelicula = mapper.Map<List<ActorPelicula>>(actorDTO);

            await repositorioPeliculas.AsignarActores(id, actorPelicula);

            return TypedResults.NoContent();
        }
    }
}
