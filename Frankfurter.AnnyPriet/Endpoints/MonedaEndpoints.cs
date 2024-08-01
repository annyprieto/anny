using AutoMapper;
using FluentValidation;
using Frankfurter.AnnyPriet.DTOS;
using Frankfurter.AnnyPriet.Entidades;
using Frankfurter.AnnyPriet.Repositorios;
using Frankfurter.AnnyPriet.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Frankfurter.AnnyPriet.Endpoints
{
    public static class MonedaEndpoints
    {
        public static RouteGroupBuilder MapMoneda(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromHours(1)).Tag("moneda-get"));
            group.MapGet("/{moneda}", ObtenerPorMonedaBase);
            group.MapPost("/", CrearMoneda).RequireAuthorization();
            group.MapPut("/{id:int}", ActualizarMoneda).RequireAuthorization();
            group.MapDelete("/{id:int}", BorrarMoneda).RequireAuthorization();

            return group;
        }

        static async Task<IResult> ObtenerTodos(IRepositorioMonedas repositorioMoneda, IMapper mapper)
        {
            var resultado = await repositorioMoneda.ObtenerTodos();

            var resultadoDTO = mapper.Map<List<MonedaDTO>>(resultado);
            return Results.Ok(resultadoDTO);
        }

        static async Task<IResult> ObtenerPorMonedaBase(string monedaBase, IRepositorioMonedas repositorioMoneda, IMapper mapper)
        {
            var resultado = await repositorioMoneda.ObtenerPorMonedaBase(monedaBase);

            if (resultado != null)
            {
                var resultadoDTO = mapper.Map<MonedaDTO>(resultado);
                return Results.Ok(resultadoDTO);
            }

            return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado} para '{monedaBase}'");
        }

        static async Task<IResult> CrearMoneda([FromBody] CrearMonedaDTO crearMonedaDTO, IRepositorioMonedas repositorioMonedas, AplicationDbContext context, IMapper mapper, IValidator<CrearMonedaDTO> validator, IOutputCacheStore outputCacheStore)
        {
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(crearMonedaDTO);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            // Verificar si la abreviatura ya existe

            var monedaExistente = await repositorioMonedas.ObtenerPorMonedaBase(crearMonedaDTO.Abreviatura);

            if (monedaExistente != null)
            {
                return Results.Conflict(MensajesDeValidacion.RegistrosExistentes);
            }

            // Crear nueva moneda
            var nuevaMoneda = mapper.Map<Moneda>(crearMonedaDTO);

            // Usar el repositorio para crear la moneda
            var abreviatura = await repositorioMonedas.Crear(nuevaMoneda);
            await outputCacheStore.EvictByTagAsync("moneda-get", default);
            var nuevaMonedaDTO = mapper.Map<MonedaDTO>(nuevaMoneda);

            return Results.Created($"/monedas/{abreviatura}", nuevaMonedaDTO);
        }

        static async Task<IResult> ActualizarMoneda(int id, [FromBody] CrearMonedaDTO crearMonedaDTO, IRepositorioMonedas repositorioMonedas, AplicationDbContext context, IMapper mapper, IValidator<CrearMonedaDTO> validator, IOutputCacheStore outputCacheStore)
        {
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(crearMonedaDTO);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var resultado = await repositorioMonedas.ObtenerPorId(id);
            if (resultado == null)
            {
                return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado} para '{id}'");
            }

            // Actualizar los datos de la moneda existente

            var monedaParaActualizar = mapper.Map<Moneda>(crearMonedaDTO);
            monedaParaActualizar.ID = id;
            try
            {
                await repositorioMonedas.Actualizar(monedaParaActualizar);
                await outputCacheStore.EvictByTagAsync("moneda-get", default);
                var monedaActualizada = mapper.Map<MonedaDTO>(monedaParaActualizar);

                return Results.Ok(new { ID = monedaActualizada, Mensaje = MensajesDeValidacion.ActualizacionExitosa });
            }
            catch (Exception)
            {
                return Results.Problem(MensajesDeValidacion.ErrorDeOperacion);
            }
        }

        static async Task<IResult> BorrarMoneda(int id, IRepositorioMonedas repositorioMonedas, IOutputCacheStore outputCacheStore)
        {
            var resultado = await repositorioMonedas.ObtenerPorId(id);

            if (resultado == null)
            {
                return Results.NotFound($"{MensajesDeValidacion.RegistroNoEncontrado} para '{id}'");
            }

            try
            {
                await repositorioMonedas.Borrar(id);
                await outputCacheStore.EvictByTagAsync("moneda-get", default);
                return Results.Ok(new { Mensaje = MensajesDeValidacion.EliminacionExitosa });
            }
            catch (Exception)
            {
                return Results.Conflict(new { Mensaje = MensajesDeValidacion.ErrorParaRegistroConVinculacionesEnOtrasTablas });
            }
        }
    }
}


