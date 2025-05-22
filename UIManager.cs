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
        private string _changeText;
        private Color _changeTextColor; 
        private DateTime _changeTextHideTime;
        private bool _showBank;
        private bool _showCash;
        private float _alpha;
        private const float FadeSpeed = 0.05f;

        private Timer _saveTimer;
        private bool _savePending;

        public UIManager(Bank bank, decimal initialCash = 0)
        {
            _bank = bank;
            _cash = initialCash;
            _changeText = string.Empty;
            _showBank = false;
            _showCash = false;
            _alpha = 0.0f;

            _saveTimer = new Timer(1000);
            _saveTimer.AutoReset = false;
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
            ShowChangeText($"+{amount:N2}", Color.FromArgb(0, 255, 0), false);
            PlaySound("LOCAL_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            ScheduleSave();
        }

        public bool RemoveCash(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount to remove cannot be negative.");

            if (_cash >= amount)
            {
                _cash -= amount;
                ShowChangeText($"-{amount:N2}", Color.FromArgb(255, 0, 0), false);
                PlaySound("PS2A_MONEY_LOST", "PALETO_SCORE_2A_BANK_SS");
                ScheduleSave();
                return true;
            }

            return false;
        }

        public void AddBank(decimal amount)
        {
            _bank.AddMoney(amount);
            ShowChangeText($"+{amount:N2}", Color.FromArgb(0, 255, 0), true);
            PlaySound("LOCAL_PLYR_CASH_COUNTER_COMPLETE", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS");
            ScheduleSave();
        }

        public bool RemoveBank(decimal amount)
        {
            if (_bank.RemoveMoney(amount))
            {
                ShowChangeText($"-{amount:N2}", Color.FromArgb(255, 0, 0), true);
                PlaySound("PS2A_MONEY_LOST", "PALETO_SCORE_2A_BANK_SS");
                ScheduleSave();
                return true;
            }

            return false;
        }

        public void ShowBothTexts()
        {
            _changeText = string.Empty;
            _showBank = true;
            _showCash = true;
            _changeTextHideTime = DateTime.Now.AddSeconds(5);
            _alpha = 0.0f;
        }

        private void ShowChangeText(string changeText, Color changeColor, bool isBank)
        {
            _changeText = changeText;
            _changeTextColor = changeColor;
            _changeTextHideTime = DateTime.Now.AddSeconds(5);
            _alpha = 0.0f;
            _showBank = isBank;
            _showCash = !isBank;
        }

        private void PlaySound(string soundName, string setName)
        {
            GTA.Audio.PlaySoundFrontendAndForget(soundName, setName);
        }

        private void ScheduleSave()
        {
            _savePending = true;
            _saveTimer.Stop();
            _saveTimer.Start();
        }

        private void OnSaveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_savePending)
            {
                BankDataManager.SaveBankData(_bank, _cash);
                _savePending = false;
            }
        }

        public void DrawUI()
        {
            if (DateTime.Now > _changeTextHideTime)
            {
                _alpha -= FadeSpeed;
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
                _alpha += FadeSpeed;
                if (_alpha > 1.0f) _alpha = 1.0f;
            }
            var resolution = Screen.Resolution;
            float xPosition = resolution.Width * 0.495f;
            float bankYPosition = (resolution.Height * 0.02f) - 20;
            float cashYPosition = bankYPosition + (resolution.Height * 0.016f);
            float changeTextYPosition = bankYPosition + (resolution.Height * 0.015f);
            if (_showBank)
            {
                TextElement bankText = new TextElement(
                    $"Bank: ${_bank.GetBalance():N2}",
                    new PointF(xPosition, bankYPosition),
                    0.6f,
                    Color.FromArgb((int)(_alpha * 255), 50, 205, 50),
                    GTA.UI.Font.ChaletComprimeCologne
                );
                bankText.Outline = true;
                bankText.Alignment = Alignment.Right;
                bankText.Draw();
                changeTextYPosition = bankYPosition + (resolution.Height * 0.015f);
            }
            if (_showCash)
            {
                float adjustedCashYPosition = _showBank ? cashYPosition : bankYPosition;

                TextElement cashText = new TextElement(
                    $"Cash: ${_cash:N2}",
                    new PointF(xPosition, adjustedCashYPosition),
                    0.6f,
                    Color.FromArgb((int)(_alpha * 255), 0, 255, 0),
                    GTA.UI.Font.ChaletComprimeCologne
                );
                cashText.Outline = true;
                cashText.Alignment = Alignment.Right;
                cashText.Draw();
                changeTextYPosition = adjustedCashYPosition + (resolution.Height * 0.015f);
            }
            if (!string.IsNullOrEmpty(_changeText))
            {
                TextElement changeText = new TextElement(
                    _changeText,
                    new PointF(xPosition, changeTextYPosition),
                    0.5f,
                    Color.FromArgb((int)(_alpha * 255), _changeTextColor.R, _changeTextColor.G, _changeTextColor.B),
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