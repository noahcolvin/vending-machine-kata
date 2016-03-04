using VendingMachine.Models;

namespace VendingMachine.Helpers
{
    public interface IProductHelper
    {
        bool ProductAvailable(IProduct product);
    }
}