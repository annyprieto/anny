using AutoMapper;
using FluentValidation;
using Frankfurter.AnnyPriet.DTOS;
using Frankfurter.AnnyPriet.Entidades;
using Frankfurter.AnnyPriet.Repositorios;
using Frankfurter.AnnyPriet.Servicios;
using Frankfurter.AnnyPriet.Utilidades;
using Microsoft.AspNetCore.OutputCaching;

namespace Frankfurter.AnnyPriet.Endpoints
{
    public static class TasaDeCambioEndpoints
    {
        public static RouteGroupBuilder MapTasaDeCambio(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromHours(1)).Tag("tasaDeCambio-get"));
            group.MapGet("/{id:int}", ObtenerPorID);
            group.MapGet("/{monedaBase}", ObtenerPorMonedaBase);
            group.MapGet("/{fecha:datetime}", ObtenerPorFecha);
            group.MapPost("/", CrearTasa).RequireAuthorization();
            group.MapPut("/{id:int}", ActualizarTasa).RequireAuthorization();
            group.MapDelete("/{id:int}", BorrarPorId).RequireAuthorization();
            group.MapDelete("/{monedaBase}", BorraPorMonedaBase).RequireAuthorization();

            return group;
        }

        static async Task<IResult> ObtenerTodos(IRepositorioTasasDeCambios repositorioTasaDeCambio, IMapper mapper)
        {
            var resultado = await repositorioTasaDeCambio.ObtenerTodos();

            var resultadoDTO = mapper.Map<List<TasaDeCambioDTO>>(resultado);
            return Results.Ok(resultadoDTO);
        }

        static async Task<IResult> ObtenerPorID(int id, IRepositorioTasasDeCambios repositorioTasasDeCambios, IMapper mapper)
        {
            var resultadoExistente = await repositorioTasasDeCambios.ObtenerPorId(id);

            if (resultadoExistente != null)
            {
                var resultadoDTO = mapper.Map<TasaDeCambioDTO>(resultadoExistente);
                return Results.Ok(resultadoDTO);
            }

            return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado}");
        }

        static async Task<IResult> ObtenerPorMonedaBase(string monedaBase, IRepositorioTasasDeCambios repositorioTasasDeCambios, IMapper mapper)
        {
            var resultadoExistente = await repositorioTasasDeCambios.ObtenerPorMonedaBase(monedaBase);

            if (resultadoExistente != null)
            {
                var resultadoDTO = mapper.Map<List<TasaDeCambioDTO>>(resultadoExistente);
                return Results.Ok(resultadoDTO);
            }

            return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado}");
        }

        static async Task<IResult> ObtenerPorFecha(DateOnly fecha, IRepositorioTasasDeCambios repositorioTasasDeCambios, IMapper mapper)
        {
            var resultadoExistente = await repositorioTasasDeCambios.ObtenerPorFecha(fecha);

            if (resultadoExistente.Any())
            {
                var resultadoDTO = mapper.Map<List<TasaDeCambioDTO>>(resultadoExistente);
                return Results.Ok(resultadoDTO);
            }

            return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado}");
        }

        static async Task<IResult> CrearTasa(string monedaBase, IFrankfurterService frankfurterService, IRepositorioTasasDeCambios repositorioTasaDeCambio, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            var resultado = await frankfurterService.ObtenerTasaDeCambioPorMonedaBase(monedaBase);

            if (resultado != null)
            {
                await repositorioTasaDeCambio.GuardarConsultaDeTasaDeCambioPorMonedaBase(resultado, monedaBase);
                await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
                return Results.Ok(resultado);
            }

            return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado} en Frankfurter para {monedaBase}");
        }

        static async Task<IResult> BorrarPorId(int id, IRepositorioTasasDeCambios repositorioTasaDeCambio, IOutputCacheStore outputCacheStore)
        {
            var resultado = await repositorioTasaDeCambio.ObtenerPorId(id);

            if (resultado == null)
            {
                return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado} para '{id}'");
            }

            await repositorioTasaDeCambio.BorrarPorId(id);
            await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
            return Results.Ok(MensajesDeValidacion.EliminacionExitosa);
        }

        static async Task<IResult> BorraPorMonedaBase(string monedaBase, IRepositorioTasasDeCambios repositorioTasasDeCambios, IOutputCacheStore outputCacheStore)
        {
            var resultadoExistente = await repositorioTasasDeCambios.BorrarPorMonedaBase(monedaBase);

            if (resultadoExistente > 0)
            {
                await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
                return Results.Ok(MensajesDeValidacion.EliminacionExitosa);
            }

            return Results.NotFound(MensajesDeValidacion.RegistroNoExistenteEnBD);
        }

        static async Task<IResult> ActualizarTasa(int id, CrearTasaDeCambioDTO crearTasaDeCambioDTO, IRepositorioTasasDeCambios repositorioTasaDeCambio, AplicationDbContext context, IMapper mapper, IValidator<CrearTasaDeCambioDTO> validator, IOutputCacheStore outputCacheStore)
        {
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(crearTasaDeCambioDTO);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var resultado = await repositorioTasaDeCambio.ObtenerPorId(id);

            if (resultado == null)
            {
                return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado} para '{id}'");
            }

            // Actualizar los datos de la moneda existente
            var tasaParaActualizar = mapper.Map<TasaDeCambio>(crearTasaDeCambioDTO);
            tasaParaActualizar.ID = id;

            try
            {
                await repositorioTasaDeCambio.Actualizar(tasaParaActualizar);
                await outputCacheStore.EvictByTagAsync("tasaDeCambio-get", default);
                var monedaActualizada = mapper.Map<TasaDeCambioDTO>(tasaParaActualizar);

                return Results.Ok(new { ID = monedaActualizada, Mensaje = MensajesDeValidacion.ActualizacionExitosa });
            }
            catch (Exception ex)
            {
                Console.WriteLine("anny");
                Console.WriteLine(ex);
                return Results.Problem(MensajesDeValidacion.ErrorDeOperacion);
            }
        }
    }
}
