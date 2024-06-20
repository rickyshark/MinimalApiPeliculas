using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApiPelicula.Context;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPelicula.Utilidades;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPelicula.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext Httpcontext;

        public RepositorioPeliculas(ApplicationDbContext dbContext, IHttpContextAccessor httpContext, IMapper mapper)
        {
            this.context = dbContext;
            this.mapper = mapper;
            this.Httpcontext = httpContext.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await Httpcontext.InsertarParametrosPaginacionEnCabecera(queryable); //
            return await queryable.OrderBy(p => p.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas
                .Include(c => c.Comentarios)
                .Include(p => p.GenerosPeliculas)
                    .ThenInclude(gp => gp.Genero)
                .Include(a => a.ActoresPeliculas.OrderBy(a => a.Orden))
                    .ThenInclude(ap =>ap.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task Actualizar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Eliminar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(p => p.Id == id);
        }

        public async Task AsignarGeneros(int id, List<int> generosId)
        {
            //Profundizar mas este metodo (Para entenderlo)

            var pelicula = await context.Peliculas
                .Include(p => p.GenerosPeliculas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe una pelicula con el id {id}");
            }

            var generosPeliculas = generosId.Select(generosId => new GeneroPelicula { GeneroId = generosId});

            pelicula.GenerosPeliculas = mapper.Map(generosPeliculas, pelicula.GenerosPeliculas); //Aqui estoy o agregando o editando o elimando genero de una pelicula.

            await context.SaveChangesAsync();

        }

        public async Task AsignarActores(int id, List<ActorPelicula> actores)
        {
            for (int i = 1; i <= actores.Count; i++)
            {
                actores[i-1].Orden = i;
            }

            var pelicula = await context.Peliculas
                                .Include(p => p.ActoresPeliculas)
                                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                throw new ArgumentException($"No existe la pelicula con id ${id}");
            }

            pelicula.ActoresPeliculas = mapper.Map(actores, pelicula.ActoresPeliculas);

            await context.SaveChangesAsync();
        }
    }
}
