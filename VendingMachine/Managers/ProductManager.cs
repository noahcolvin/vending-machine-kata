using System.Collections.Generic;
using VendingMachine.Models;

namespace VendingMachine.Managers
{
    public class ProductManager : IProductManager
    {
        public Dictionary<IProduct, int> Inventory { get; } = new Dictionary<IProduct, int>();
        
        public bool ProductAvailable(IProduct product)
        {
            return Inventory.ContainsKey(product) && Inventory[product] > 0;
        }

        public void AddStock(IProduct product, int quantity)
        {
            if (!Inventory.ContainsKey(product))
                Inventory.Add(product, quantity);
            else
                Inventory[product] += quantity;
        }

        public void RemoveProduct(IProduct product)
        {
            Inventory[product]--;
        }
    }
}