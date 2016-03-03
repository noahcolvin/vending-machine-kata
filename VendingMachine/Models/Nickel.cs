namespace VendingMachine.Models
{
    public class Nickel : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.Large;
        public decimal Value { get; set; } = 0.05M;
    }
}