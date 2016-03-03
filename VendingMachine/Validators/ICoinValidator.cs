using VendingMachine.Models;

namespace VendingMachine.Validators
{
    public interface ICoinValidator
    {
        bool CoinValid(ICoin coin);
        decimal CoinValue(ICoin coin);
    }
}