using AutoMapper;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPelicula.Utilidades
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            //Mapeo de mantenimiento Genero
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();

            //Mapeo de mantenimiento Actor
            CreateMap<CrearActorDTO, Actor>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore());

            CreateMap<Actor, ActorDTO>();

            //Mapeo de mantenimiento Pelicula
            CreateMap<CrearPeliculaDTO, Pelicula>()
                .ForMember(p => p.Poster, opciones => opciones.Ignore());
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(p => p.Generos, opciones => opciones
                    .MapFrom(p => p.GenerosPeliculas
                    .Select(gp => new GeneroDTO { Id = gp.GeneroId, Nombre = gp.Genero.Nombre })))
                .ForMember(p => p.Actores, opciones => opciones
                    .MapFrom(p => p.ActoresPeliculas
                    .Select(ap => new ActorPeliculaDTO { Id = ap.ActorId, Nombre = ap.Actor.Nombre, Personaje = ap.personaje })));

            //Mapeo de mantenimiento Comentario
            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();
        }
    }
}
