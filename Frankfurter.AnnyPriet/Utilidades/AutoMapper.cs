using AutoMapper;
using Frankfurter.AnnyPriet.DTOS;
using Frankfurter.AnnyPriet.Entidades;

namespace Frankfurter.AnnyPriet.Utilidades
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<CrearMonedaDTO, Moneda>();
            CreateMap<Moneda, MonedaDTO>();

            CreateMap<CrearTasaDeCambioDTO, TasaDeCambio>();
            CreateMap<TasaDeCambio, TasaDeCambioDTO>();
        }
    }
}
