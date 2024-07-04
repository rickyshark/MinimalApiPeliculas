using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPelicula.Repositorios;
using MinimalApiPeliculas.Filtros;

namespace MinimalApiPelicula.EndPoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {

            group.MapGet("/", ObtenerGeneros)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get")).RequireAuthorization();
            group.MapGet("/{id:int}", ObtenerPorId);//.AddEndpointFilter<FiltroDePrueba>();
            group.MapPost("/", NuevoGenero).AddEndpointFilter<FiltrosValidaciones<CrearGeneroDTO>>()
                .RequireAuthorization("esadmin");
            group.MapPut("/{id:int}", ActualizarGenero).AddEndpointFilter<FiltrosValidaciones<CrearGeneroDTO>>()
                .RequireAuthorization("esadmin");
            group.MapDelete("/{id:int}", EliminarGenero)
                .RequireAuthorization("esadmin");

            return group;
        }
        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodos();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);
            return TypedResults.Ok(generosDTO);
        }


        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerPorId(int id, IRepositorioGeneros repositorio, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            var generoDTO = mapper.Map<GeneroDTO>(genero);

            return TypedResults.Ok(generoDTO);
        }

        static async Task<Results<Created<GeneroDTO>,ValidationProblem>> NuevoGenero(CrearGeneroDTO generoDto, IRepositorioGeneros repositorio, 
            IOutputCacheStore outputCache, IMapper mapper)
        {

            var genero = mapper.Map<Genero>(generoDto);

            int id = await repositorio.Crear(genero);
            await outputCache.EvictByTagAsync("generos-get", default); //Limpiando cache a la hora de crear un nuevo genero.

            var generoDTO = mapper.Map<GeneroDTO>(genero);


            return TypedResults.Created($"/generos/{id}", generoDTO); //201
        }

        static async Task<Results<NoContent, NotFound, ValidationProblem>> ActualizarGenero(int id, CrearGeneroDTO generoDTO, IRepositorioGeneros repositorio, 
            IOutputCacheStore outputCache, IMapper mapper)
        {

            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = mapper.Map<Genero>(generoDTO);
            genero.Id = id;

            await repositorio.Actualizar(genero);
            await outputCache.EvictByTagAsync("generos-get", default); //Limpiando cache luego de actualizar un genero.
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> EliminarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCache)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCache.EvictByTagAsync("generos-get", default); //Limpiando cache luego de eliminar un genero.
            return TypedResults.NoContent();
        }
    }
}
