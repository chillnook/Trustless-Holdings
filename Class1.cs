using System;
using GTA;
using GTA.Native;
using GTA.UI;
using GTA.Math;
using System.Windows.Forms;
using Screen = GTA.UI.Screen;
using Control = GTA.Control;

namespace TrustlessHoldingsInc
{
    public class Main : Script
    {
        private Bank _bank;
        private UIManager _uiManager;

        public Main()
        {
            _bank = new Bank(1000); // Initialize with a starting bank balance of 1000
            _uiManager = new UIManager(_bank, 500); // Initialize with a starting cash balance of 500

            decimal cash = 0; // Temporary variable to load cash balance
            BankDataManager.LoadBankData(_bank, ref cash); // Load saved bank and cash data
            _uiManager = new UIManager(_bank, cash); // Initialize UIManager with loaded cash balance

            // Initialize the EconomyAPI
            EconomyAPI.Instance.Initialize(_uiManager);

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAborted; // Save data when the script is stopped
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Always hide the default in-game cash value
            Function.Call(Hash.DISPLAY_CASH, false);

            // Hide the ammo count in the top-right corner only when the custom UI is visible
            if (_uiManager.IsUIVisible())
            {
                Function.Call(Hash.DISPLAY_AMMO_THIS_FRAME, false);
            }

            // Draw the custom UI on every tick
            _uiManager.DrawUI();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Modifiers == Keys.Shift) // Add money to cash (SHIFT + INSERT)
            {
                _uiManager.AddCash(500);
            }
            else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Shift) // Remove money from cash (SHIFT + DELETE)
            {
                if (!_uiManager.RemoveCash(200))
                {
                    Screen.ShowSubtitle("Not enough cash.");
                }
            }
            else if (e.KeyCode == Keys.Insert) // Add money to the bank
            {
                _uiManager.AddBank(500);
            }
            else if (e.KeyCode == Keys.Delete) // Remove money from the bank
            {
                if (!_uiManager.RemoveBank(200))
                {
                    Screen.ShowSubtitle("Not enough balance in the bank.");
                }
            }
            else if (e.KeyCode == Keys.Multiply) // Add cash
            {
                _uiManager.AddCash(100);
            }
            else if (e.KeyCode == Keys.Divide) // Remove cash
            {
                if (!_uiManager.RemoveCash(50))
                {
                    Screen.ShowSubtitle("Not enough cash.");
                }
            }
            else if (e.KeyCode == Keys.Z) // Show both bank and cash texts
            {
                _uiManager.ShowBothTexts();
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            // Save bank and cash data when the script is stopped
            BankDataManager.SaveBankData(_bank, _uiManager.GetCash());
        }
    }
}
