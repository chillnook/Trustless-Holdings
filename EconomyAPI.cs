using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustlessHoldingsInc
{
    /// <summary>
    /// Provides an API for managing the economy system, including bank and cash balances.
    /// </summary>
    public class EconomyAPI
    {
        private static EconomyAPI _instance;

        private decimal _bankBalance;
        private decimal _cashBalance;

        // Events for bank-related actions
        /// <summary>
        /// Triggered when the bank balance changes.
        /// </summary>
        public event Action<decimal> OnBankBalanceChanged;

        /// <summary>
        /// Triggered when money is added to the bank.
        /// </summary>
        public event Action<decimal> OnBankMoneyAdded;

        /// <summary>
        /// Triggered when money is removed from the bank.
        /// </summary>
        public event Action<decimal> OnBankMoneyRemoved;

        // Events for cash-related actions
        /// <summary>
        /// Triggered when the cash balance changes.
        /// </summary>
        public event Action<decimal> OnCashBalanceChanged;

        /// <summary>
        /// Triggered when money is added to cash.
        /// </summary>
        public event Action<decimal> OnCashMoneyAdded;

        /// <summary>
        /// Triggered when money is removed from cash.
        /// </summary>
        public event Action<decimal> OnCashMoneyRemoved;

        /// <summary>
        /// Singleton instance of the EconomyAPI.
        /// </summary>
        public static EconomyAPI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EconomyAPI();
                }
                return _instance;
            }
        }

        private EconomyAPI()
        {
            // Initialize balances (default values can be adjusted as needed)
            _bankBalance = 0;
            _cashBalance = 0;
        }

        /// <summary>
        /// Sets the initial balances for the bank and cash accounts.
        /// </summary>
        /// <param name="bankBalance">The initial bank balance.</param>
        /// <param name="cashBalance">The initial cash balance.</param>
        public void Initialize(decimal bankBalance, decimal cashBalance)
        {
            _bankBalance = bankBalance;
            _cashBalance = cashBalance;

            // Trigger initial balance events
            OnBankBalanceChanged?.Invoke(_bankBalance);
            OnCashBalanceChanged?.Invoke(_cashBalance);
        }

        /// <summary>
        /// Adds money to the bank.
        /// </summary>
        /// <param name="amount">The amount of money to add.</param>
        public void AddToBank(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to add cannot be negative.");

            _bankBalance += amount;
            OnBankMoneyAdded?.Invoke(amount);
            OnBankBalanceChanged?.Invoke(_bankBalance);
        }

        /// <summary>
        /// Removes money from the bank.
        /// </summary>
        /// <param name="amount">The amount of money to remove.</param>
        /// <returns>True if the money was successfully removed; otherwise, false.</returns>
        public bool RemoveFromBank(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to remove cannot be negative.");

            if (_bankBalance >= amount)
            {
                _bankBalance -= amount;
                OnBankMoneyRemoved?.Invoke(amount);
                OnBankBalanceChanged?.Invoke(_bankBalance);
                return true;
            }

            return false; // Not enough balance
        }

        /// <summary>
        /// Adds money to cash.
        /// </summary>
        /// <param name="amount">The amount of money to add.</param>
        public void AddToCash(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to add cannot be negative.");

            _cashBalance += amount;
            OnCashMoneyAdded?.Invoke(amount);
            OnCashBalanceChanged?.Invoke(_cashBalance);
        }

        /// <summary>
        /// Removes money from cash.
        /// </summary>
        /// <param name="amount">The amount of money to remove.</param>
        /// <returns>True if the money was successfully removed; otherwise, false.</returns>
        public bool RemoveFromCash(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to remove cannot be negative.");

            if (_cashBalance >= amount)
            {
                _cashBalance -= amount;
                OnCashMoneyRemoved?.Invoke(amount);
                OnCashBalanceChanged?.Invoke(_cashBalance);
                return true;
            }

            return false; // Not enough cash
        }

        /// <summary>
        /// Gets the current bank balance.
        /// </summary>
        /// <returns>The current bank balance.</returns>
        public decimal GetBankBalance()
        {
            return _bankBalance;
        }

        /// <summary>
        /// Gets the current cash balance.
        /// </summary>
        /// <returns>The current cash balance.</returns>
        public decimal GetCashBalance()
        {
            return _cashBalance;
        }
    }
}
