using Frankfurter.AnnyPriet.Entidades;

namespace Frankfurter.AnnyPriet.Repositorios
{
    public interface IRepositorioMonedas
    {
        Task<List<Moneda>> ObtenerTodos();
        Task<Moneda?> ObtenerPorId(int id);
        Task<Moneda?> ObtenerPorMonedaBase(string monedaBase);
        Task<string> Crear(Moneda dBMoneda);
        Task<int> Actualizar(Moneda moneda);
        Task<int> Borrar(int id);
        Task GuardarConsultaDeListaDeMonedas(Dictionary<string, string> monedas);
    }
}