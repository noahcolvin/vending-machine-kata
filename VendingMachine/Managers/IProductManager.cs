using VendingMachine.Models;

namespace VendingMachine.Managers
{
    public interface IProductManager
    {
        bool ProductAvailable(IProduct product);
    }
}