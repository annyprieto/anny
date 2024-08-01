using System.ComponentModel.DataAnnotations;

namespace Frankfurter.AnnyPriet.Entidades
{
    public class Moneda
    {
        public int ID { get; set; }

        [StringLength(3, MinimumLength = 3)]
        public string Abreviatura { get; set; } = null!;

        [StringLength(25, MinimumLength = 4)]
        public string Descripcion { get; set; } = null!;

        public List<TasaDeCambio> MonedasFrom { get; set; } = new List<TasaDeCambio>();
        public List<TasaDeCambio> MonedasTo { get; set; } = new List<TasaDeCambio>();
    }
}
