namespace VendingMachine.Models
{
    public class Nickel : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.Large;
    }
}