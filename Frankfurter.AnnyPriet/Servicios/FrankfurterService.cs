using Frankfurter.AnnyPriet.Entidades;
using System.Text.Json;

namespace Frankfurter.AnnyPriet.Servicios
{
    public class FrankfurterService : IFrankfurterService
    {
        private readonly HttpClient _httpClient;

        public FrankfurterService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://api.frankfurter.app");
        }

        async Task<Dictionary<string, string>?> IFrankfurterService.ObtenerListaDeMonedas()
        {
            var response = await _httpClient.GetAsync($"/currencies");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, string>>(content);
        }

        async Task<TasaDeCambioJson?> IFrankfurterService.ObtenerTasaDeCambioPorMonedaBase(string monedaBase)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/latest?from={monedaBase}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TasaDeCambioJson>(content);
            }
            catch { return null; }
        }

        async Task<TasaDeCambioJson?> IFrankfurterService.ObtenerTasaDeCambioEntreMonedas(string monedaFrom, string monedaTo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/latest?from={monedaFrom}&to={monedaTo}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TasaDeCambioJson>(content);
            }
            catch { return null; }
        }

        async Task<TasaDeCambioJson?> IFrankfurterService.ObtenerCalculoDeConversionEntreMonedas(string monedaFrom, string monedaTo, decimal monto)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/latest?amount={monto}&from={monedaFrom}&to={monedaTo}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TasaDeCambioJson>(content);
            }
            catch { return null; }
        }

        async Task<TasaDeCambioJson?> IFrankfurterService.ObtenerTasasDeCambioHistoricas(string monedaFrom, string monedaTo, string fecha)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/{fecha:YYYY-MM-DD}?from={monedaFrom}&to={monedaTo}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TasaDeCambioJson>(content);
            }
            catch { return null; }
        }
    }
}
