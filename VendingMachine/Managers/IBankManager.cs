namespace VendingMachine.Managers
{
    public interface IBankManager
    {
        decimal Amount { get; }
        void AddMoney(decimal amount);
    }
}