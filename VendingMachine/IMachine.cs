using System.Collections.Generic;
using VendingMachine.Models;

namespace VendingMachine
{
    public interface IMachine
    {
        string Display { get; }
        void InsertCoin(ICoin coin);
        List<ICoin> CoinReturn { get; }
        void SelectProduct(IProduct product);
        IProduct DispensedProduct { get; }
        void ReturnCoins();
    }
}