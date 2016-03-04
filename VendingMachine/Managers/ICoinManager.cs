using System.Collections.Generic;
using VendingMachine.Models;

namespace VendingMachine.Managers
{
    public interface ICoinManager
    {
        bool CoinValid(ICoin coin);
        decimal CoinValueByWeight(ICoin coin);
        IEnumerable<ICoin> DistributeChange(decimal amount);
    }
}