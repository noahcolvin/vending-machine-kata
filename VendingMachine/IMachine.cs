using System.Collections.Generic;
using VendingMachine.Models;

namespace VendingMachine
{
    public interface IMachine
    {
        string Display { get; set; }
        void InsertCoin(ICoin coin);
        List<ICoin> CoinReturn { get; }
    }
}