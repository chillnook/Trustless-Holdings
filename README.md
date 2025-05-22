# Trustless Holdings Inc. - Economy API

## Introduction

The **Economy API** is a simple and efficient C# library designed to manage bank and cash balances. It integrates seamlessly into GTA V mods using Script Hook V .NET.

---

## Features

- **Bank and Cash Management**
  - Add or remove money from bank and cash accounts.
  - View current balances.

- **Event Notifications**
  - Get notified when balances are updated.
  - React to money being added or removed.

- **Easy Setup**
  - Singleton design for simple integration.
  - Save and load balances automatically.

---

## Getting Started

### Requirements

- [Script Hook V .NET Nightly](https://github.com/scripthookvdotnet/scripthookvdotnet-nightly/releases)
- GTA V game client

### Installation

1. Download the required files.
2. Place `TrustlessHoldingsInc.dll` into the `scripts` folder of your GTA V directory.
3. Ensure Script Hook V .NET is properly installed.

---

## Usage

### Initialization

Set up the Economy API with starting bank and cash balances:

```csharp
EconomyAPI.Instance.Initialize(1000, 500);
```

---

### Adding Money

Add money to the bank or cash:

```csharp
// Add $500 to the bank
EconomyAPI.Instance.AddToBank(500);

// Add $200 to cash
EconomyAPI.Instance.AddToCash(200);
```

---

### Removing Money

Remove money from the bank or cash:

```csharp
// Remove $300 from the bank
if (!EconomyAPI.Instance.RemoveFromBank(300))
{
    Screen.ShowSubtitle("Not enough balance in the bank.");
}

// Remove $100 from cash
if (!EconomyAPI.Instance.RemoveFromCash(100))
{
    Screen.ShowSubtitle("Not enough cash.");
}
```

---

### Checking Balances

Retrieve the current bank and cash balances:

```csharp
decimal bankBalance = EconomyAPI.Instance.GetBankBalance();
decimal cashBalance = EconomyAPI.Instance.GetCashBalance();
Screen.ShowSubtitle($"Bank: ${bankBalance}, Cash: ${cashBalance}");
```

---

### Events

Subscribe to balance-related events:

```csharp
EconomyAPI.Instance.OnBankBalanceChanged += balance =>
    Screen.ShowSubtitle($"Bank balance updated: ${balance}");

EconomyAPI.Instance.OnCashBalanceChanged += balance =>
    Screen.ShowSubtitle($"Cash balance updated: ${balance}");
```

---

## License

This project is licensed under the MIT License. See `LICENSE` for details.
