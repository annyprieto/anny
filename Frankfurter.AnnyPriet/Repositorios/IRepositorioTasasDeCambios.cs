using Frankfurter.AnnyPriet.Entidades;

namespace Frankfurter.AnnyPriet.Repositorios
{
    public interface IRepositorioTasasDeCambios
    {
        Task<List<TasaDeCambio>> ObtenerTodos();
        Task<TasaDeCambio?> ObtenerPorId(int id);
        Task<List<TasaDeCambio>> ObtenerPorMonedaBase(string monedaBase);
        Task<List<TasaDeCambio>> ObtenerPorFecha(DateOnly fecha);
        Task<int> Actualizar(TasaDeCambio dBTasasDeCambio);
        Task<int> BorrarPorId(int id);
        Task<int> BorrarPorMonedaBase(string monedaBase);
        Task GuardarConsultaDeTasaDeCambioPorMonedaBase(TasaDeCambioJson apiTasasDeCambio, string monedaBase);
        Task GuardarConsultaDeTasaDeCambioEntreMonedas(TasaDeCambioJson apiTasasDeCambio, string monedaFrom, string monedaTo);
    }
}