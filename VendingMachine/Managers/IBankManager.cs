namespace VendingMachine.Managers
{
    public interface IBankManager
    {
        decimal Amount { get; set; }
        void AddMoney(decimal amount);
    }
}