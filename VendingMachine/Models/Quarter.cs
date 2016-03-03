namespace VendingMachine.Models
{
    public class Quarter : ICoin
    {
        public CoinSize Size { get; set; } = CoinSize.XLarge;
        public decimal Value { get; set; } = 0.25M;
    }
}