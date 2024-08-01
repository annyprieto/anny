using FluentValidation;
using Frankfurter.AnnyPriet.Utilidades;

namespace Frankfurter.AnnyPriet.DTOS
{
    public class CrearMonedaDTO
    {
        public string Abreviatura { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }

    public class CrearMonedaDTOValidator : AbstractValidator<CrearMonedaDTO>
    {
        public CrearMonedaDTOValidator()
        {
            RuleFor(x => x.Abreviatura).NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido)
                .MinimumLength(3).WithMessage(MensajesDeValidacion.LongitudMinima)
                .MaximumLength(3).WithMessage(MensajesDeValidacion.LongitudMaxima);

            RuleFor(x => x.Descripcion).NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido)
                .MinimumLength(3).WithMessage(MensajesDeValidacion.LongitudMinima)
                .MaximumLength(25).WithMessage(MensajesDeValidacion.LongitudMaxima);
        }
    }
}
