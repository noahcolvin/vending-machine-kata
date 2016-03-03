using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using VendingMachine.Models;
using VendingMachine.Validators;

namespace VendingMachine
{
    public class Machine : IMachine
    {
        private readonly ICoinValidator _coinValidator;
        private decimal _currentInsertedValue;

        public List<ICoin> CoinReturn { get; }= new List<ICoin>();
        private string _display = "INSERT COIN";

        public string Display
        {
            get
            {
                if (_currentInsertedValue != 0)
                    return $"{_currentInsertedValue:C}";

                return _display;
            }
            set { _display = value; }
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
        }
    }
}