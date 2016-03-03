namespace VendingMachine.Models
{
    public class Dime : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.Small;
    }
}