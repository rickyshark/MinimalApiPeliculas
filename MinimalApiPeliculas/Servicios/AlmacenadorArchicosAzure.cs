
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MinimalApiPelicula.Servicios
{
    public class AlmacenadorArchicosAzure : IAlmacenadorArchivo
    {

        private string connectioString;
        public AlmacenadorArchicosAzure(IConfiguration configuration)
        {
            connectioString = configuration.GetConnectionString("AzureStore")!;
        }
        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectioString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(PublicAccessType.Blob);

            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var blob = cliente.GetBlobClient(nombreArchivo);

            var blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = archivo.ContentType;
            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);

            return blob.Uri.ToString();
        }

        public async Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            var cliente = new BlobContainerClient(connectioString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var nombreArchivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(nombreArchivo);

            await blob.DeleteIfExistsAsync();
        }
    }
}
