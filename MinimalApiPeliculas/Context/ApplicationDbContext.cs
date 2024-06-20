using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using MinimalApiPelicula.Entidades;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPelicula.Context
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genero>().Property(g => g.Nombre).HasMaxLength(100);

            modelBuilder.Entity<Actor>().Property(a => a.Nombre).HasMaxLength(250);
            modelBuilder.Entity<Actor>().Property(a => a.Foto).IsUnicode();

            modelBuilder.Entity<Pelicula>().Property(p => p.Titulo).HasMaxLength(250);
            modelBuilder.Entity<Pelicula>().Property(p => p.Poster).IsUnicode();

            modelBuilder.Entity<GeneroPelicula>().HasKey(g => new { g.GeneroId, g.PeliculaId });
            modelBuilder.Entity<ActorPelicula>().HasKey(ap => new { ap.PeliculaId, ap.ActorId });

            modelBuilder.Entity<IdentityUser>().ToTable("Usuarios");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims"); 
            //Un Claims es una informacion acerca de alguien o algo. Aqui de roles. Aqui puedo asignar permiso a todos los usuarios que tengan un rol en especifico.
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsuriosClaims");
            //Un Claims es una informacion acerca de alguien o algo. Aqui de usuarios. Aqui puedo asignar permiso a todos los usuarios que tengan un rol en especifico.
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsuariosLogins");
            //Distinto logion de usuarios. Puede iniciar sesion con google o faceboook.
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsuariosRoles");
            //Tabla intermedia entre usuarios y roles.
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsuariosToken");
            //Guardar token de autenticacion de los usuarios
        }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }

        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<GeneroPelicula> GenerosPeliculas { get; set; }
        public DbSet<ActorPelicula> ActoresPeliculas { get; set; }
        public DbSet<Error> Errores { get; set; }

    }
}
