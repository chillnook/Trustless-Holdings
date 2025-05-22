using GTA.Native;
using GTA.UI;
using System;
using System.Drawing;
using System.Timers;

namespace TrustlessHoldingsInc
{
    public class UIManager
    {
        private Bank _bank;
        private decimal _cash;
        private string _changeText; // Temporary change text (e.g., +100 or -100)
        private Color _changeTextColor; // Color for the change text
        private DateTime _changeTextHideTime; // When to hide the change text
        private bool _showBank; // Whether to show the bank text
        private bool _showCash; // Whether to show the cash text
        private float _alpha; // Current alpha value for fade-in/out
        private const float FadeSpeed = 0.05f; // Speed of fade-in/out

        private Timer _saveTimer; // Timer for debounced saving
        private bool _savePending; // Whether a save is pending

        public UIManager(Bank bank, decimal initialCash = 0)
        {
            _bank = bank;
            _cash = initialCash;
            _changeText = string.Empty;
            _showBank = false; // Default to hiding both texts
            _showCash = false;
            _alpha = 0.0f; // Start fully transparent

            // Initialize the save timer
            _saveTimer = new Timer(1000); // 1 second debounce interval
            _saveTimer.AutoReset = false; // Only trigger once per activation
            _saveTimer.Elapsed += OnSaveTimerElapsed;
        }

        public decimal GetBankBalance()
        {
            return _bank.GetBalance();
        }

        public decimal GetCash()
        {
            return _cash;
        }

        public void AddCash(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to add cannot be negative.");

            _cash += amount;
            ShowChangeText($"+{amount:N2}", Color.FromArgb(0, 255, 0), false); // Green for positive change

            // Play sound effect for adding cash
            PlaySound("LOCAL_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");

            // Schedule a save
            ScheduleSave();
        }

        public bool RemoveCash(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to remove cannot be negative.");

            if (_cash >= amount)
            {
                _cash -= amount;
                ShowChangeText($"-{amount:N2}", Color.FromArgb(255, 0, 0), false); // Red for negative change

                // Play sound effect for removing cash
                PlaySound("PS2A_MONEY_LOST", "PALETO_SCORE_2A_BANK_SS");

                // Schedule a save
                ScheduleSave();
                return true;
            }

            return false; // Not enough cash
        }

        public void AddBank(decimal amount)
        {
            _bank.AddMoney(amount);
            ShowChangeText($"+{amount:N2}", Color.FromArgb(0, 255, 0), true); // Green for positive change

            // Play sound effect for adding to the bank
            PlaySound("LOCAL_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");

            // Schedule a save
            ScheduleSave();
        }

        public bool RemoveBank(decimal amount)
        {
            if (_bank.RemoveMoney(amount))
            {
                ShowChangeText($"-{amount:N2}", Color.FromArgb(255, 0, 0), true); // Red for negative change

                // Play sound effect for removing from the bank
                PlaySound("PS2A_MONEY_LOST", "PALETO_SCORE_2A_BANK_SS");

                // Schedule a save
                ScheduleSave();
                return true;
            }

            return false; // Not enough balance
        }

        public void ShowBothTexts()
        {
            _changeText = string.Empty; // No change text for this case
            _showBank = true;
            _showCash = true;
            _changeTextHideTime = DateTime.Now.AddSeconds(5); // Show for 5 seconds
            _alpha = 0.0f; // Start fade-in
        }

        private void ShowChangeText(string changeText, Color changeColor, bool isBank)
        {
            _changeText = changeText;
            _changeTextColor = changeColor;
            _changeTextHideTime = DateTime.Now.AddSeconds(5); // Show for 5 seconds
            _alpha = 0.0f; // Start fade-in

            // Show only the relevant text (bank or cash)
            _showBank = isBank;
            _showCash = !isBank;
        }

        private void PlaySound(string soundName, string setName)
        {
            GTA.Audio.PlaySoundFrontendAndForget(soundName, setName);
        }

        private void ScheduleSave()
        {
            _savePending = true; // Mark that a save is pending
            _saveTimer.Stop(); // Reset the timer
            _saveTimer.Start(); // Start the timer
        }

        private void OnSaveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_savePending)
            {
                // Perform the save
                BankDataManager.SaveBankData(_bank, _cash);
                _savePending = false; // Clear the pending save flag
            }
        }

        public void DrawUI()
        {
            // Fade out if the change text has expired
            if (DateTime.Now > _changeTextHideTime)
            {
                _alpha -= FadeSpeed; // Gradually decrease alpha
                if (_alpha <= 0.0f)
                {
                    _alpha = 0.0f;
                    _showBank = false;
                    _showCash = false;
                    return;
                }
            }
            else
            {
                // Fade in
                _alpha += FadeSpeed;
                if (_alpha > 1.0f) _alpha = 1.0f; // Cap alpha at 1.0f
            }

            // Get the screen resolution
            var resolution = Screen.Resolution;

            // Calculate positions dynamically for the top-right corner
            float xPosition = resolution.Width * 0.495f;
            float bankYPosition = (resolution.Height * 0.02f) - 20; // Bank text position
            float cashYPosition = bankYPosition + (resolution.Height * 0.016f); // Cash text below bank text
            float changeTextYPosition = bankYPosition + (resolution.Height * 0.015f); // Change text slightly below the main text

            // Draw bank text if visible
            if (_showBank)
            {
                TextElement bankText = new TextElement(
                    $"Bank: ${_bank.GetBalance():N2}",
                    new PointF(xPosition, bankYPosition),
                    0.6f,
                    Color.FromArgb((int)(_alpha * 255), 50, 205, 50), // Apply alpha to color
                    GTA.UI.Font.ChaletComprimeCologne
                );
                bankText.Outline = true;
                bankText.Alignment = Alignment.Right;
                bankText.Draw();

                // Adjust change text position to be slightly below the bank text
                changeTextYPosition = bankYPosition + (resolution.Height * 0.015f); // Slightly closer to the bank text
            }

            // Draw cash text if visible
            if (_showCash)
            {
                // Adjust Y position if both texts are visible
                float adjustedCashYPosition = _showBank ? cashYPosition : bankYPosition;

                TextElement cashText = new TextElement(
                    $"Cash: ${_cash:N2}",
                    new PointF(xPosition, adjustedCashYPosition),
                    0.6f,
                    Color.FromArgb((int)(_alpha * 255), 0, 255, 0), // Apply alpha to color
                    GTA.UI.Font.ChaletComprimeCologne
                );
                cashText.Outline = true;
                cashText.Alignment = Alignment.Right;
                cashText.Draw();

                // Adjust change text position to be slightly below the cash text
                changeTextYPosition = adjustedCashYPosition + (resolution.Height * 0.015f); // Slightly closer to the cash text
            }

            // Draw change text if applicable
            if (!string.IsNullOrEmpty(_changeText))
            {
                TextElement changeText = new TextElement(
                    _changeText,
                    new PointF(xPosition, changeTextYPosition),
                    0.5f,
                    Color.FromArgb((int)(_alpha * 255), _changeTextColor.R, _changeTextColor.G, _changeTextColor.B), // Apply alpha to color
                    GTA.UI.Font.ChaletComprimeCologne
                );
                changeText.Outline = true;
                changeText.Alignment = Alignment.Right;
                changeText.Draw();
            }
        }
        public bool IsUIVisible()
        {
            return _showBank || _showCash;
        }
    }
}