namespace MinimalApiPeliculas.Validaciones
{
    public class Utilidades
    {
        public static string CampoMensajeObligatorio = "El campo {PropertyName} es obligatorio";
        public static string CampoMaximoLengthObligatorio = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string PrimeraLetraMayuscula = "El campo {PropertyName} debe empezar en mayuscula";
        public static string MensajeEmail = "El campo {PropertyName} debe ser un email válido";
        public static string GreaterThanOrEqualToMensaje(DateTime fechaMinima)
        {
            return "El campo {PropertyName} debe ser posterio a: " + fechaMinima.ToString("yyyy-MM-dd");
        }
        public static bool PrimeraLetraEnMayuscula(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var primeraLetra = valor[0].ToString();
            return primeraLetra == primeraLetra.ToUpper();
        }
    }
}
