namespace VendingMachine.Models
{
    public class Penny : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.Medium;
        public decimal Value { get; set; } = 0.01M;
    }
}