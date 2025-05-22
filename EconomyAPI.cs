using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrustlessHoldingsInc
{
    /// <summary>
    /// Provides an API for interacting with the economy system, including bank and cash balances.
    /// </summary>
    public class EconomyAPI
    {
        private static EconomyAPI _instance;
        private UIManager _uiManager;

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

        // Event for UI visibility
        /// <summary>
        /// Triggered when the custom UI is shown.
        /// </summary>
        public event Action OnUIShown;

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

        private EconomyAPI() { }

        /// <summary>
        /// Initializes the API with the provided UIManager instance.
        /// </summary>
        /// <param name="uiManager">The UIManager instance to use for the economy system.</param>
        /// <exception cref="ArgumentNullException">Thrown if the UIManager is null.</exception>
        public void Initialize(UIManager uiManager)
        {
            _uiManager = uiManager ?? throw new ArgumentNullException(nameof(uiManager));
        }

        /// <summary>
        /// Adds money to the bank.
        /// </summary>
        /// <param name="amount">The amount of money to add.</param>
        public void AddToBank(decimal amount)
        {
            _uiManager.AddBank(amount);
            OnBankMoneyAdded?.Invoke(amount);
            OnBankBalanceChanged?.Invoke(_uiManager.GetBankBalance());
        }

        /// <summary>
        /// Removes money from the bank.
        /// </summary>
        /// <param name="amount">The amount of money to remove.</param>
        /// <returns>True if the money was successfully removed; otherwise, false.</returns>
        public bool RemoveFromBank(decimal amount)
        {
            bool success = _uiManager.RemoveBank(amount);
            if (success)
            {
                OnBankMoneyRemoved?.Invoke(amount);
                OnBankBalanceChanged?.Invoke(_uiManager.GetBankBalance());
            }
            return success;
        }

        /// <summary>
        /// Adds money to cash.
        /// </summary>
        /// <param name="amount">The amount of money to add.</param>
        public void AddToCash(decimal amount)
        {
            _uiManager.AddCash(amount);
            OnCashMoneyAdded?.Invoke(amount);
            OnCashBalanceChanged?.Invoke(_uiManager.GetCash());
        }

        /// <summary>
        /// Removes money from cash.
        /// </summary>
        /// <param name="amount">The amount of money to remove.</param>
        /// <returns>True if the money was successfully removed; otherwise, false.</returns>
        public bool RemoveFromCash(decimal amount)
        {
            bool success = _uiManager.RemoveCash(amount);
            if (success)
            {
                OnCashMoneyRemoved?.Invoke(amount);
                OnCashBalanceChanged?.Invoke(_uiManager.GetCash());
            }
            return success;
        }

        /// <summary>
        /// Gets the current bank balance.
        /// </summary>
        /// <returns>The current bank balance.</returns>
        public decimal GetBankBalance()
        {
            return _uiManager.GetBankBalance();
        }

        /// <summary>
        /// Gets the current cash balance.
        /// </summary>
        /// <returns>The current cash balance.</returns>
        public decimal GetCashBalance()
        {
            return _uiManager.GetCash();
        }

        /// <summary>
        /// Shows the custom UI for the economy system.
        /// </summary>
        public void ShowUI()
        {
            _uiManager.ShowBothTexts();
            OnUIShown?.Invoke();
        }
    }
}
