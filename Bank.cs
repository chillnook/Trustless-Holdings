using System;

namespace TrustlessHoldingsInc
{
    public class Bank
    {
        private decimal _balance;

        public Bank(decimal initialBalance = 0)
        {
            _balance = initialBalance;
        }

        public decimal GetBalance()
        {
            return _balance;
        }

        public void AddMoney(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to add cannot be negative.");

            _balance += amount;
        }

        public bool RemoveMoney(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to remove cannot be negative.");

            if (_balance >= amount)
            {
                _balance -= amount;
                return true;
            }

            return false; // Not enough balance
        }
    }
}
