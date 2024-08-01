using FluentValidation;
using Frankfurter.AnnyPriet.Utilidades;

namespace Frankfurter.AnnyPriet.DTOS
{
    public class CrearTasaDeCambioDTO
    {
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public int MonedaFromID { get; set; }  // llave foranea
        public int MonedaToID { get; set; }  // llave foranea
        public decimal Rate { get; set; }
    }

    public class CrearTasaDeCambioDTOValidator : AbstractValidator<CrearTasaDeCambioDTO>
    {
        public CrearTasaDeCambioDTOValidator()
        {
            RuleFor(x => x.Date).GreaterThanOrEqualTo(new DateOnly(1999, 1, 04))
                .WithMessage(MensajesDeValidacion.FechaMinima);

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido)
                .GreaterThan(0).WithMessage(MensajesDeValidacion.ValorMayorACero);

            RuleFor(x => x.MonedaFromID).NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido)
                .GreaterThan(0).WithMessage(MensajesDeValidacion.ValorMayorACero);

            RuleFor(x => x.MonedaToID).NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido)
                .GreaterThan(0).WithMessage(MensajesDeValidacion.ValorMayorACero);

            RuleFor(x => x.Rate)
                .NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido)
                .GreaterThan(0).WithMessage(MensajesDeValidacion.ValorMayorACero);
        }
    }
}
