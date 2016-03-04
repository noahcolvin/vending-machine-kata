using System.Collections.Generic;
using System.Linq;
using VendingMachine.Models;

namespace VendingMachine.Managers
{
    public class CoinManager : ICoinManager
    {
        private readonly List<ICoin> _coins = new List<ICoin>
        {
            new Quarter(),
            new Dime(),
            new Nickel(),
            new Penny()
        };

        public bool CoinValid(ICoin coin)
        {
            return CoinValueByWeight(coin) > 0;
        }

        public decimal CoinValueByWeight(ICoin coin)
        {
            var value = _coins.Find(c => c.Size == coin.Size).Value;

            return value > 0.01M ? value : decimal.Zero;
        }

        public IEnumerable<ICoin> DistributeChange(decimal amount)
        {
            var change = new List<ICoin>();

            while (amount > 0)
            {
                var coin = _coins.FindAll(c => c.Value <= amount).OrderByDescending(c => c.Value).First();
                change.Add(coin);
                amount -= coin.Value;
            }

            return change;
        }
    }
}