using Frankfurter.AnnyPriet.Repositorios;
using Frankfurter.AnnyPriet.Servicios;
using Frankfurter.AnnyPriet.Utilidades;
using Microsoft.AspNetCore.OutputCaching;

namespace Frankfurter.AnnyPriet.Endpoints
{
    public static class FrankfurterEndpoints
    {
        public static RouteGroupBuilder MapFrankfurter(this RouteGroupBuilder group)
        {
            group.MapGet("/monedas", ObtenerListaDeMonedas).RequireAuthorization();
            group.MapGet("/tasa/{monedaBase}", ObtenerTasasDeCambioPorMonedaBase).RequireAuthorization();
            group.MapGet("/tasa/{monedaFrom}/{monedaTo}", ObtenerTasasDeCambioEntreMonedas).RequireAuthorization();
            group.MapGet("/tasa/{monedaFrom}/{monedaTo}/{monto}", ObtenerCalculoDeConversionEntreMonedas).RequireAuthorization();
            group.MapGet("/tasa/historico/{monedaFrom}/{monedaTo}/{fecha}", ObtenerTasasDeCambioHistoricas).RequireAuthorization();

            return group;
        }

        static async Task<IResult> ObtenerListaDeMonedas(IFrankfurterService frankfurterService, IRepositorioMonedas repositorioMonedas, IOutputCacheStore outputCacheStore)
        {
            var resultado = await frankfurterService.ObtenerListaDeMonedas();

            await repositorioMonedas.GuardarConsultaDeListaDeMonedas(resultado);
            await outputCacheStore.EvictByTagAsync("moneda-get", default);
            return Results.Ok(resultado);
        }

        static async Task<IResult> ObtenerTasasDeCambioPorMonedaBase(string monedaBase, IFrankfurterService frankfurterService, IRepositorioTasasDeCambios repositorioTasaDeCambio, IOutputCacheStore outputCacheStore)
        {
            var resultado = await frankfurterService.ObtenerTasaDeCambioPorMonedaBase(monedaBase);

            if (resultado != null)
            {
                await repositorioTasaDeCambio.GuardarConsultaDeTasaDeCambioPorMonedaBase(resultado, monedaBase);
                await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
                return Results.Ok(resultado);
            }

            return Results.NotFound(MensajesDeValidacion.RegistroNoEncontrado);
        }

        static async Task<IResult> ObtenerTasasDeCambioEntreMonedas(string monedaFrom, string monedaTo, IFrankfurterService frankfurterService, IRepositorioTasasDeCambios repositorioTasaDeCambio, IOutputCacheStore outputCacheStore)
        {
            var resultado = await frankfurterService.ObtenerTasaDeCambioEntreMonedas(monedaFrom, monedaTo);

            if (resultado != null)
            {
                await repositorioTasaDeCambio.GuardarConsultaDeTasaDeCambioEntreMonedas(resultado, monedaFrom, monedaTo);
                await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
                return Results.Ok(resultado);
            }

            return Results.NotFound(MensajesDeValidacion.RegistroNoEncontrado);
        }

        static async Task<IResult> ObtenerCalculoDeConversionEntreMonedas(string monedaFrom, string monedaTo, decimal monto, IFrankfurterService frankfurterService, IRepositorioTasasDeCambios repositorioTasaDeCambio, IOutputCacheStore outputCacheStore)
        {
            var resultado = await frankfurterService.ObtenerCalculoDeConversionEntreMonedas(monedaFrom, monedaTo, monto);

            if (resultado != null)
            {
                await repositorioTasaDeCambio.GuardarConsultaDeTasaDeCambioEntreMonedas(resultado, monedaFrom, monedaTo);
                await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
                return Results.Ok(resultado);
            }

            return Results.NotFound(MensajesDeValidacion.RegistroNoEncontrado);
        }

        static async Task<IResult> ObtenerTasasDeCambioHistoricas(string monedaFrom, string monedaTo, string fecha, IFrankfurterService frankfurterService, IRepositorioTasasDeCambios repositorioTasaDeCambio, IOutputCacheStore outputCacheStore)
        {
            var resultado = await frankfurterService.ObtenerTasasDeCambioHistoricas(monedaFrom, monedaTo, fecha);

            if (resultado != null)
            {
                await repositorioTasaDeCambio.GuardarConsultaDeTasaDeCambioEntreMonedas(resultado, monedaFrom, monedaTo);
                await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
                return Results.Ok(resultado);
            }

            return Results.NotFound(MensajesDeValidacion.RegistroNoEncontrado);
        }
    }
}

