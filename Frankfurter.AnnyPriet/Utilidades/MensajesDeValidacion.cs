namespace Frankfurter.AnnyPriet.Utilidades
{
    public class MensajesDeValidacion
    {
        // Mensajes para campos
        public static string CampoRequerido = "Este campo es requerido";
        public static string LongitudMaxima = "El campo {PropertyName} debe tener máximo {MaxLength} caracteres.";
        public static string LongitudMinima = "El campo {PropertyName} debe tener mínimo {MinLength} caracteres.";
        public static string ValorMayorACero = "El campo {PropertyName} debe ser mayor a cero.";
        public static string FechaMinima = "El campo {PropertyName} debe ser mayor a 1999-01-04";
        public static string EmailMensaje = "El campo debe contener un email valido";

        // Mensajes fallidos de operacion
        public static string TablaSinRegistrosEnBD = "La tabla no posee registros registros";
        public static string RegistrosExistentes = "El registro ya existe";
        public static string RegistroNoExistenteEnBD = "El registro no existe";
        public static string RegistroNoEncontrado = "No se encontraron resultados";
        public static string AlmacenamientoFallido = "El registro no ha sido almacenado exitosamente";
        public static string ErrorDeOperacion = "Se ha presentado un error al realizar la operación";
        public static string ErrorParaRegistroConVinculacionesEnOtrasTablas = "Se ha presentado un error al realizar la operación. El registro puede tener vinculaciones en otras tablas";

        // Mensajes exitosos de operacion
        public static string ActualizacionExitosa = "El registro ha sido actualizado exitosamente";
        public static string EliminacionExitosa = "El registro ha sido eliminado exitosamente";
    }
}
