using Frankfurter.AnnyPriet.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Frankfurter.AnnyPriet.Repositorios
{
    public class RepositorioMonedas : IRepositorioMonedas
    {
        private readonly AplicationDbContext context;

        public RepositorioMonedas(AplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Moneda>> ObtenerTodos()
        {
            return await context.Monedas.OrderBy(x => x.ID).ToListAsync();
        }

        public async Task<Moneda?> ObtenerPorId(int id)
        {
            return await context.Monedas.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task<Moneda?> ObtenerPorMonedaBase(string monedaBase)
        {
            return await context.Monedas.FirstOrDefaultAsync(t => t.Abreviatura == monedaBase);
        }

        public async Task<string> Crear(Moneda dBMoneda)
        {
            context.Add(dBMoneda);
            await context.SaveChangesAsync();
            return dBMoneda.Abreviatura;
        }

        public async Task<int> Actualizar(Moneda moneda)
        {
            context.Update(moneda);
            await context.SaveChangesAsync();
            return moneda.ID;
        }

        public async Task<int> Borrar(int id)
        {
            return await context.Monedas.Where(a => a.ID == id).ExecuteDeleteAsync();
        }

        public async Task GuardarConsultaDeListaDeMonedas(Dictionary<string, string> monedas)
        {
            // Obtener todas las monedas existentes
            var monedasExistentes = await context.Monedas.ToDictionaryAsync(m => m.Abreviatura, m => m);

            foreach (var moneda in monedas)
            {
                if (!monedasExistentes.ContainsKey(moneda.Key))
                {
                    // Añadir nueva moneda solo si no existe
                    var nuevaMoneda = new Moneda
                    {
                        Abreviatura = moneda.Key,
                        Descripcion = moneda.Value
                    };
                    context.Monedas.Add(nuevaMoneda);
                }
                // Si la moneda ya existe, no hacemos nada
            }

            await context.SaveChangesAsync();
        }
    }
}