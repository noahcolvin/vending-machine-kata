using System.Collections.Generic;
using VendingMachine.Models;

namespace VendingMachine.Helpers
{
    public interface ICoinHelper
    {
        bool CoinValid(ICoin coin);
        decimal CoinValueByWeight(ICoin coin);
        IEnumerable<ICoin> DistributeChange(decimal amount);
    }
}