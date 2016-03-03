namespace VendingMachine.Models
{
    public interface ICoin
    {
        CoinSize Size { get; set; } 
        decimal Value { get; set; }
    }
}