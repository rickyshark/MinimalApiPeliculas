using Microsoft.IdentityModel.Tokens;

namespace MinimalApiPeliculas.Utilidades
{
    public static class Llaves
    {
        public const string IssuerPropio = "miapp-07";
        private const string SeccionLlaves = "Authentication:Schemes:Bearer:SigningKeys";
        private const string SeccionLlavesEmisor = "Issuer";
        private const string SeccionLlavesValor = "Value";

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration) => ObtenerLlave(configuration, IssuerPropio);

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration, string isssur)
        {
            var singingKey = configuration.GetSection(SeccionLlaves)
                .GetChildren()
                .SingleOrDefault(llave => llave[SeccionLlavesEmisor] == isssur);

            if (singingKey is not null && singingKey[SeccionLlavesValor] is string valorLlave)
            {
                yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
            }
        }

        public static IEnumerable<SecurityKey> ObtenerTodasLlaves(IConfiguration configuration)
        {
            var singingKeys = configuration.GetSection(SeccionLlaves)
                .GetChildren();

            foreach (var signingKey in singingKeys)
            {
                if (signingKey[SeccionLlavesValor] is string valorLlave)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLlave));
                }
                
            }

        }
    }
}
