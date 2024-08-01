namespace Frankfurter.AnnyPriet.Entidades
{
    public class TasaDeCambio
    {
        public int ID { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public int MonedaFromID { get; set; }  // llave foranea
        public int MonedaToID { get; set; }  // llave foranea
        public decimal Rate { get; set; }

        public Moneda MonedaFrom { get; set; } = null!;
        public Moneda MonedaTo { get; set; } = null!;
    }
}
