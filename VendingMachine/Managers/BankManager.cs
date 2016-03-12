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
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Cannot have negative amount");

                _amount = value;
            }
        }

        public void AddMoney(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException("Cannot have negative amount");

            Amount += amount;
        }
    }
}