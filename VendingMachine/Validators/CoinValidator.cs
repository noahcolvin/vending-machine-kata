using VendingMachine.Models;

namespace VendingMachine.Validators
{
    public class CoinValidator : ICoinValidator
    {
        public bool CoinValid(ICoin coin)
        {
            return CoinValue(coin) > 0;
        }

        public decimal CoinValue(ICoin coin)
        {
            switch (coin.Size)
            {
                case CoinSize.Small:
                    return 0.10M;
                case CoinSize.Large:
                    return 0.05M;
                case CoinSize.XLarge:
                    return 0.25M;
                default:
                    return decimal.Zero;
            }
        }
    }
}