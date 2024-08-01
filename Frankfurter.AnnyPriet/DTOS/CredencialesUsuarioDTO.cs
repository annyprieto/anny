using FluentValidation;
using Frankfurter.AnnyPriet.Utilidades;

namespace Frankfurter.AnnyPriet.DTOS
{
    public class CredencialesUsuarioDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class CredencialesUsuarioDTOValidator : AbstractValidator<CredencialesUsuarioDTO>
    {
        public CredencialesUsuarioDTOValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido)
                .MaximumLength(250).WithMessage(MensajesDeValidacion.LongitudMaxima)
                .EmailAddress().WithMessage(MensajesDeValidacion.EmailMensaje);

            RuleFor(x => x.Password).NotEmpty().WithMessage(MensajesDeValidacion.CampoRequerido);
        }
    }
}
