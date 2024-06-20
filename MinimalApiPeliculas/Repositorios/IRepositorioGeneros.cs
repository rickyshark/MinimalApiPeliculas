﻿using MinimalApiPelicula.Entidades;

namespace MinimalApiPelicula.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<List<Genero>> ObtenerTodos();
        Task<Genero?> ObtenerPorId(int id);
        Task<int> Crear(Genero genero);
        Task<bool> Existe(int id);
        Task Actualizar(Genero genero);
        Task Borrar(int id);
        Task<List<int>> Existen(List<int> ids);
        Task<bool> Existe(int id, string nombre);
    }
}
