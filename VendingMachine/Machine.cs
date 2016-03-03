using System.Collections.Generic;
using VendingMachine.Helpers;
using VendingMachine.Models;

namespace VendingMachine
{
    public class Machine : IMachine
    {
        private readonly ICoinHelper _coinHelper;
        private decimal _currentInsertedValue;
        private const string DefaultDisplayValue = "INSERT COIN";
        private string _display = DefaultDisplayValue;
        private IProduct _selectedProduct;

        public List<ICoin> CoinReturn { get; } = new List<ICoin>();
        public IProduct DispensedProduct { get; private set; }

        public string Display
        {
            get
            {
                if (_display == DefaultDisplayValue && _currentInsertedValue != 0)
                    return $"{_currentInsertedValue:C}";

                var currentValue = _display;
                _display = DefaultDisplayValue;

                return currentValue;
            }
            private set
            {
                _display = value;
            }
        }

        public Machine(ICoinHelper coinHelper)
        {
            _coinHelper = coinHelper;
        }

        public void InsertCoin(ICoin coin)
        {
            if (_coinHelper.CoinValid(coin))
                _currentInsertedValue += _coinHelper.CoinValueByWeight(coin);
            else
                CoinReturn.Add(coin);

            VerifyProductAmount();
        }

        public void SelectProduct(IProduct product)
        {
            _selectedProduct = product;

            VerifyProductAmount();
        }

        private void VerifyProductAmount()
        {
            if (_selectedProduct == null)
                return;

            if (_selectedProduct.Price <= _currentInsertedValue)
            {
                DispensedProduct = _selectedProduct;
                _currentInsertedValue = _currentInsertedValue - _selectedProduct.Price;
                Display = "THANK YOU";

                if (_currentInsertedValue > 0)
                {
                    CoinReturn.AddRange(_coinHelper.DistributeChange(_currentInsertedValue));
                    _currentInsertedValue = 0;
                }
            }
            else
                Display = $"PRICE {_selectedProduct.Price:C}";
        }
    }
}