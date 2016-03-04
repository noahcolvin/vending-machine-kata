using System.Collections.Generic;
using System.Linq;
using VendingMachine.Helpers;
using VendingMachine.Models;

namespace VendingMachine
{
    public class Machine : IMachine
    {
        private readonly ICoinHelper _coinHelper;
        private readonly IProductHelper _productHelper;
        private const string DefaultDisplayValue = "INSERT COIN";
        private string _display = DefaultDisplayValue;
        private IProduct _selectedProduct;
        private readonly List<ICoin> _insertedCoins = new List<ICoin>();

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

        public Machine(ICoinHelper coinHelper, IProductHelper productHelper)
        {
            _coinHelper = coinHelper;
            _productHelper = productHelper;
        }

        public void InsertCoin(ICoin coin)
        {
            if (_coinHelper.CoinValid(coin))
            {
                coin.Value = _coinHelper.CoinValueByWeight(coin);
                _insertedCoins.Add(coin);
            }
            else
                CoinReturn.Add(coin);

            VerifyAndVendProduct();
        }

        public void SelectProduct(IProduct product)
        {
            if (!_productHelper.ProductAvailable(product))
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
                DispensedProduct = _selectedProduct;
                Display = "THANK YOU";

                var remainingBalance = CurrentInsertedValue - _selectedProduct.Price;
                _insertedCoins.Clear();

                if (remainingBalance > 0)
                    CoinReturn.AddRange(_coinHelper.DistributeChange(remainingBalance));
            }
            else
                Display = $"PRICE {_selectedProduct.Price:C}";
        }
    }
}