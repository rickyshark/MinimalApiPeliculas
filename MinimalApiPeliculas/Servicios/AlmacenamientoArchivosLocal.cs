
namespace MinimalApiPelicula.Servicios
{
    public class AlmacenamientoArchivosLocal : IAlmacenadorArchivo
    {
        private readonly IHttpContextAccessor httpAccesor;
        private readonly IWebHostEnvironment env;

        public AlmacenamientoArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpAccesor)
        {
            this.httpAccesor = httpAccesor;
            this.env = env;
        }
        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);
            using (var ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                var contenido = ms.ToArray();
                await File.WriteAllBytesAsync(ruta, contenido);
            }

            var url = $"{httpAccesor.HttpContext!.Request.Scheme}://{httpAccesor.HttpContext.Request.Host}";

            var urlArchivo = Path.Combine(url, contenedor,nombreArchivo).Replace("\\","/");

            return urlArchivo;

        }

        public Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return Task.CompletedTask;
            }

            var nombreArhcivo = Path.GetFileName(ruta);
            var directorioArchivo = Path.Combine(env.WebRootPath,contenedor,nombreArhcivo);
            if (File.Exists(directorioArchivo))
            {
                File.Delete(directorioArchivo);
            }

            return Task.CompletedTask;

        }
    }
}
