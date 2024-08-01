using Frankfurter.AnnyPriet.Entidades;
using Frankfurter.AnnyPriet.Utilidades;
using Microsoft.EntityFrameworkCore;

namespace Frankfurter.AnnyPriet.Repositorios
{
    public class RepositorioTasasDeCambios : IRepositorioTasasDeCambios
    {
        private readonly AplicationDbContext context;
        private readonly IRepositorioMonedas repositorioMonedas;

        public RepositorioTasasDeCambios(AplicationDbContext context, IRepositorioMonedas repositorioMonedas)
        {
            this.context = context;
            this.repositorioMonedas = repositorioMonedas;
        }

        public async Task<List<TasaDeCambio>> ObtenerTodos()
        {
            return await context.TasasDeCambios.OrderBy(x => x.MonedaFromID).ToListAsync();
        }

        public async Task<TasaDeCambio?> ObtenerPorId(int id)
        {
            return await context.TasasDeCambios.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task<List<TasaDeCambio>> ObtenerPorMonedaBase(string monedaBase)
        {
            var monedaBaseExistente = await repositorioMonedas.ObtenerPorMonedaBase(monedaBase);

            if (monedaBaseExistente == null)
            {
                Console.WriteLine(MensajesDeValidacion.RegistroNoExistenteEnBD);
                return new List<TasaDeCambio>();
            }

            return await context.TasasDeCambios.Where(t => t.MonedaFromID == monedaBaseExistente.ID).ToListAsync();
        }

        public async Task<List<TasaDeCambio>> ObtenerPorFecha(DateOnly fecha)
        {
            return await context.TasasDeCambios.Where(t => t.Date == fecha).ToListAsync();
        }

        public async Task<int> Actualizar(TasaDeCambio dBTasasDeCambio)
        {
            context.Update(dBTasasDeCambio);
            await context.SaveChangesAsync();
            return dBTasasDeCambio.ID;
        }

        public async Task<int> BorrarPorId(int id)
        {
            return await context.TasasDeCambios.Where(a => a.ID == id).ExecuteDeleteAsync();
        }

        public async Task<int> BorrarPorMonedaBase(string monedaBase)
        {
            var monedaBaseExistente = await repositorioMonedas.ObtenerPorMonedaBase(monedaBase);

            if (monedaBaseExistente == null)
            {
                Console.WriteLine(MensajesDeValidacion.RegistroNoExistenteEnBD);
                return 0;
            }

            return await context.TasasDeCambios.Where(a => a.MonedaFromID == monedaBaseExistente.ID).ExecuteDeleteAsync();
        }

        public async Task GuardarConsultaDeTasaDeCambioPorMonedaBase(TasaDeCambioJson apiTasasDeCambio, string monedaBase)
        {
            // Verificar si la monedabase existe en la tabla monedas de la BD

            var monedaBaseExistente = await repositorioMonedas.ObtenerPorMonedaBase(monedaBase);

            if (monedaBaseExistente == null)
            {
                Console.WriteLine(MensajesDeValidacion.RegistroNoExistenteEnBD);
                return;
            }

            // Iniciar una transacción. Incluye verificacion de los registros de moneda existente en BD (Solo Rate y MonedasTo porque amount siempre es 1)
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                foreach (var rate in apiTasasDeCambio.Rates)
                {
                    // Verificacion
                    var existingRatesSet = await context.TasasDeCambios
                     .Where(t => t.MonedaFromID == monedaBaseExistente.ID &&
                                 t.Rate == rate.Value &&
                                 t.Date == apiTasasDeCambio.Date)
                    .Join(context.Monedas,
                          t => t.MonedaToID,
                          t2 => t2.ID,
                          (t, t2) => new { A = t, B = t2 })
                    .Select(x => new
                    {
                        PropiedadDeA = x.A.Rate,
                        PropiedadDeB = x.B.Abreviatura,
                    }).ToListAsync();

                    // Guardado de modenas que no existen en BD

                    if (existingRatesSet.Count == 0)
                    {
                        var monedaTargetExistente = await repositorioMonedas.ObtenerPorMonedaBase(rate.Key);

                        if (monedaTargetExistente == null)
                        {
                            Console.WriteLine(MensajesDeValidacion.RegistroNoExistenteEnBD);
                            continue;
                        }

                        // Crear un nuevo registro
                        context.TasasDeCambios.Add(new TasaDeCambio
                        {
                            MonedaFromID = monedaBaseExistente.ID,
                            MonedaToID = monedaTargetExistente.ID,
                            Date = apiTasasDeCambio.Date,
                            Amount = (decimal)apiTasasDeCambio.Amount,
                            Rate = (decimal)rate.Value
                        });
                    }
                    else
                    {
                        // Log de monedas no existentes
                        Console.WriteLine($"Descripcion destino {rate.Key} no encontrada en la base de datos.");
                    }
                }

                //Guardar cambios y commit de la transacción
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback de la transacción en caso de error
                await transaction.RollbackAsync();
                throw new Exception(MensajesDeValidacion.ErrorDeOperacion, ex);
            }
        }

        public async Task GuardarConsultaDeTasaDeCambioEntreMonedas(TasaDeCambioJson apiTasasDeCambio, string monedaFrom, string monedaTo)
        {
            // Verificar si las monedas existen en la tabla monedas de la BD
            var monedaFromExistente = await repositorioMonedas.ObtenerPorMonedaBase(monedaFrom);
            if (monedaFromExistente == null)
            {
                Console.WriteLine(MensajesDeValidacion.RegistroNoExistenteEnBD);
                return;
            }
            // Verificar si las monedas existen en la tabla de registros 
            try
            {
                foreach (var rate in apiTasasDeCambio.Rates)
                {
                    var monedaToExistente = await repositorioMonedas.ObtenerPorMonedaBase(rate.Key);
                    if (monedaToExistente == null)
                    {
                        throw new Exception(MensajesDeValidacion.RegistroNoExistenteEnBD);
                    }
                    var tasaExistente = await context.TasasDeCambios
                        .FirstOrDefaultAsync(t => t.MonedaFromID == monedaFromExistente.ID &&
                                                  t.MonedaToID == monedaToExistente.ID &&
                                                  t.Rate == rate.Value &&
                                                  t.Date == apiTasasDeCambio.Date &
                                                  t.Amount == apiTasasDeCambio.Amount);
                    if (tasaExistente == null)
                    {
                        // Crear un nuevo registro solo si no existe
                        context.TasasDeCambios.Add(new TasaDeCambio
                        {
                            MonedaFromID = monedaFromExistente.ID,
                            MonedaToID = monedaToExistente.ID,
                            Date = apiTasasDeCambio.Date,
                            Amount = apiTasasDeCambio.Amount,
                            Rate = apiTasasDeCambio.Rates[monedaTo]
                        });
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MensajesDeValidacion.AlmacenamientoFallido, ex);
            }
        }
    }
}





