using Frankfurter.AnnyPriet.Entidades;

namespace Frankfurter.AnnyPriet.Servicios
{
    public interface IFrankfurterService
    {
        Task<Dictionary<string, string>?> ObtenerListaDeMonedas();
        Task<TasaDeCambioJson?> ObtenerTasaDeCambioPorMonedaBase(string monedaBase);
        Task<TasaDeCambioJson?> ObtenerCalculoDeConversionEntreMonedas(string monedaFrom, string monedaTo, decimal monto);
        Task<TasaDeCambioJson?> ObtenerTasaDeCambioEntreMonedas(string monedaFrom, string monedaTo);
        Task<TasaDeCambioJson?> ObtenerTasasDeCambioHistoricas(string monedaFrom, string monedaTo, string fecha);
    }
}