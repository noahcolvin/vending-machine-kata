using System.Collections.Generic;
using VendingMachine.Models;

namespace VendingMachine.Managers
{
    public interface IProductManager
    {
        bool ProductAvailable(IProduct product);
        void AddStock(IProduct product, int quantity);
        void RemoveProduct(IProduct product);
        Dictionary<IProduct, int> Inventory { get; }
    }
}