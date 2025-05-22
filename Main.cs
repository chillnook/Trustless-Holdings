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
            _bank = new Bank(1000);
            _uiManager = new UIManager(_bank, 500);

            decimal cash = 0;
            BankDataManager.LoadBankData(_bank, ref cash);
            _uiManager = new UIManager(_bank, cash);

            EconomyAPI.Instance.Initialize(_bank.GetBalance(), cash);

            Tick += OnTick;
            KeyDown += OnKeyDown;
            Aborted += OnAborted; 
        }

        private void OnTick(object sender, EventArgs e)
        {
            Function.Call(Hash.DISPLAY_CASH, false);

            if (_uiManager.IsUIVisible())
            {
                Function.Call(Hash.DISPLAY_AMMO_THIS_FRAME, false);
            }

            _uiManager.DrawUI();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && e.Modifiers == Keys.Shift)
            {
                _uiManager.AddCash(500);
            }
            else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Shift) 
            {
                if (!_uiManager.RemoveCash(200))
                {
                    Screen.ShowSubtitle("Not enough cash.");
                }
            }
            else if (e.KeyCode == Keys.Insert) 
            {
                _uiManager.AddBank(500);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (!_uiManager.RemoveBank(200))
                {
                    Screen.ShowSubtitle("Not enough balance in the bank.");
                }
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                _uiManager.AddCash(100);
            }
            else if (e.KeyCode == Keys.Divide)
            {
                if (!_uiManager.RemoveCash(50))
                {
                    Screen.ShowSubtitle("Not enough cash.");
                }
            }
            else if (e.KeyCode == Keys.Z)
            {
                _uiManager.ShowBothTexts();
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            BankDataManager.SaveBankData(_bank, _uiManager.GetCash());
        }
    }
}
