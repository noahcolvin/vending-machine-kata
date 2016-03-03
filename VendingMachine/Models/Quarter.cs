namespace VendingMachine.Models
{
    public class Quarter : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.XLarge;
    }
}