using System.Collections.Generic;
using System.Linq;
using VendingMachine.Managers;
using VendingMachine.Models;

namespace VendingMachine
{
    public class Machine : IMachine
    {
        private readonly ICoinManager _coinManager;
        private readonly IProductManager _productManager;
        private readonly IBankManager _bankManager;
        private string _display;
        private IProduct _selectedProduct;
        private readonly List<ICoin> _insertedCoins = new List<ICoin>();
        private string DefaultDisplayValue => CanMakeChange() ? "INSERT COIN" : "EXACT CHANGE ONLY";

        private decimal CurrentInsertedValue
        {
            get { return _insertedCoins.Sum(c => c.Value); }
        }

        public List<ICoin> CoinReturn { get; } = new List<ICoin>();
        public IProduct DispensedProduct { get; private set; }

        public string Display
        {
            get
            {
                _display = _display ?? DefaultDisplayValue;

                if (_display == DefaultDisplayValue && CurrentInsertedValue != 0)
                    return $"{CurrentInsertedValue:C}";

                var currentValue = _display;
                _display = DefaultDisplayValue;

                return currentValue;
            }
            private set
            {
                _display = value;
            }
        }

        public Machine(ICoinManager coinManager, IProductManager productManager, IBankManager bankManager)
        {
            _coinManager = coinManager;
            _productManager = productManager;
            _bankManager = bankManager;
        }

        public void InsertCoin(ICoin coin)
        {
            if (_coinManager.CoinValid(coin))
            {
                coin.Value = _coinManager.CoinValueByWeight(coin);
                _insertedCoins.Add(coin);
            }
            else
                CoinReturn.Add(coin);

            VerifyAndVendProduct();
        }

        public void SelectProduct(IProduct product)
        {
            if (!_productManager.ProductAvailable(product))
            {
                Display = "SOLD OUT";
                return;
            }

            _selectedProduct = product;

            VerifyAndVendProduct();
        }

        public void ReturnCoins()
        {
            CoinReturn.AddRange(_insertedCoins);
            _insertedCoins.Clear();
        }

        private void VerifyAndVendProduct()
        {
            if (_selectedProduct == null)
                return;

            if (_selectedProduct.Price <= CurrentInsertedValue)
            {
                _productManager.RemoveProduct(_selectedProduct);
                _bankManager.AddMoney(_selectedProduct.Price);

                DispensedProduct = _selectedProduct;
                Display = "THANK YOU";

                var remainingBalance = CurrentInsertedValue - _selectedProduct.Price;
                _insertedCoins.Clear();

                if (remainingBalance > 0)
                    CoinReturn.AddRange(_coinManager.DistributeChange(remainingBalance));
            }
            else
                Display = $"PRICE {_selectedProduct.Price:C}";
        }

        private bool CanMakeChange()
        {
            return _productManager.Inventory.Where(p => p.Value > 0).Any(p => p.Key.Price < _bankManager.Amount);
        }
    }
}