using System.Collections.Generic;
using VendingMachine.Models;
using VendingMachine.Validators;

namespace VendingMachine
{
    public class Machine : IMachine
    {
        private readonly ICoinValidator _coinValidator;
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

        public Machine(ICoinValidator coinValidator)
        {
            _coinValidator = coinValidator;
        }

        public void InsertCoin(ICoin coin)
        {
            if (_coinValidator.CoinValid(coin))
                _currentInsertedValue += _coinValidator.CoinValue(coin);
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

            if (_selectedProduct.Price == _currentInsertedValue)
            {
                DispensedProduct = _selectedProduct;
                _currentInsertedValue = decimal.Zero;
                Display = "THANK YOU";
            }
            else
                Display = $"PRICE {_selectedProduct.Price:C}";
        }
    }
}