namespace Frankfurter.AnnyPriet.DTOS
{
    public class TasaDeCambioDTO
    {
        public int ID { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public int MonedaFromID { get; set; }  // llave foranea
        public int MonedaToID { get; set; }  // llave foranea
        public decimal Rate { get; set; }
    }
}
