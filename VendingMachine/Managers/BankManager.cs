using System;

namespace VendingMachine.Managers
{
    public class BankManager : IBankManager
    {
        private decimal _amount;

        public decimal Amount
        {
            get
            {
                return _amount;
            }
            private set
            {
                _amount = ValidateAmount(value);
            }
        }

        public void AddMoney(decimal amount)
        {
            Amount += ValidateAmount(amount);
        }

        private decimal ValidateAmount(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            return amount;
        }
    }
}