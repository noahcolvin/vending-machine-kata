namespace VendingMachine.Models
{
    public class Dime : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.Small;
        public decimal Value { get; set; } = 0.10M;
    }
}