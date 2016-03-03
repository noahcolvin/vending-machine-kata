namespace VendingMachine.Models
{
    public class Penny : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.Medium;
    }
}