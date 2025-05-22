using System;
using System.IO;
using System.Xml.Serialization;

namespace TrustlessHoldingsInc
{
    public static class BankDataManager
    {
        private static readonly string SaveDirectory = Path.Combine("scripts", "TrustlessHoldings");
        private static readonly string SaveFile = Path.Combine(SaveDirectory, "data.xml");

        public static void SaveBankData(Bank bank, decimal cash)
        {
            try
            {
                if (!Directory.Exists(SaveDirectory))
                {
                    Directory.CreateDirectory(SaveDirectory);
                }

                using (var writer = new StreamWriter(SaveFile))
                {
                    var serializer = new XmlSerializer(typeof(BankData));
                    var data = new BankData
                    {
                        BankBalance = bank.GetBalance(),
                        CashBalance = cash
                    };
                    serializer.Serialize(writer, data);
                }
            }
            catch (Exception ex)
            {
                GTA.UI.Screen.ShowSubtitle($"Error saving data: {ex.Message}");
            }
        }

        public static void LoadBankData(Bank bank, ref decimal cash)
        {
            try
            {
                if (File.Exists(SaveFile))
                {
                    using (var reader = new StreamReader(SaveFile))
                    {
                        var serializer = new XmlSerializer(typeof(BankData));
                        var data = (BankData)serializer.Deserialize(reader);
                        bank.AddMoney(data.BankBalance - bank.GetBalance()); 
                        cash = data.CashBalance;
                    }
                }
            }
            catch (Exception ex)
            {
                GTA.UI.Screen.ShowSubtitle($"Error loading data: {ex.Message}");
            }
        }

        [Serializable]
        public class BankData
        {
            public decimal BankBalance { get; set; }
            public decimal CashBalance { get; set; }
        }
    }
}
