﻿using Microsoft.EntityFrameworkCore;
using MinimalApiPelicula.Context;
using MinimalApiPelicula.DTOs;
using MinimalApiPelicula.Entidades;
using MinimalApiPelicula.Utilidades;

namespace MinimalApiPelicula.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioActores(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDto)
        {
            var queryable = context.Actores.AsQueryable();

            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            return await queryable.OrderBy(a => a.Nombre).Paginar(paginacionDto).ToListAsync();
        }

        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await context.Actores.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Actor>> ObtenerPorNombre(string nombre)
        {
            return await context.Actores.Where(a => a.Nombre.Contains(nombre)).OrderBy(a => a.Nombre).ToListAsync();
        }

        public async Task<int> Crear(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();

            return actor.Id;
        }

        public async Task Actualizar(Actor actor)
        {
            context.Update(actor);
            await context.SaveChangesAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Actores.AnyAsync(a => a.Id == id);
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await context.Actores.Where(a => ids.Contains(a.Id)).Select(a => a.Id).ToListAsync();
        }
        public async Task Eliminar(int id)
        {
            await context.Actores.Where(a => a.Id == id).ExecuteDeleteAsync();
        }

    }
}
