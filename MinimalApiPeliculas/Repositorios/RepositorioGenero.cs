using Microsoft.EntityFrameworkCore;
using MinimalApiPelicula.Context;
using MinimalApiPelicula.Entidades;

namespace MinimalApiPelicula.Repositorios
{
    public class RepositorioGenero : IRepositorioGeneros
    {
        private readonly ApplicationDbContext context;
   
        public RepositorioGenero(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<int> Crear(Genero genero)
        {
            context.Add(genero);
            await context.SaveChangesAsync();

            return genero.Id;
        }

        public async Task Actualizar(Genero genero)
        {
            context.Update(genero);
            await context.SaveChangesAsync();
        }
        public async Task<bool> Existe(int id)
        {
            return await context.Generos.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Existe(int id, string nombre)
        {
            return await context.Generos.AnyAsync(x => x.Id != id && x.Nombre == nombre);
        }
        public async Task<List<int>> Existen(List<int> ids)
        {

            //Profundizar mas este metodo (Para entenderlo)
            return await context.Generos
                .Where(g => ids.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {
            return await context.Generos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Genero>> ObtenerTodos()
        {
            return await context.Generos.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }
}
