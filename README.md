# Snack Vending Machine

A C# backend implementation of an automated snack vending machine system.

The project demonstrates Object-Oriented Programming principles, SOLID design, the State Pattern, data structure selection, payment validation, change calculation, exception handling, and automated testing.

## Requirements

- .NET 8.0 or later
- C#
- xUnit for automated testing
The project was developed using .NET 10.

## Features

- 5 × 5 vending machine inventory grid
- Numeric keypad item selection
- Snack stock management
- USD payment validation
- Coin payments
- Note payments
- Card payments
- Exact card payment validation
- Balance tracking
- Change calculation
- Stack-based cash storage
- Greedy change calculation with fallback combination search
- State Pattern for machine workflow
- Exception handling for invalid operations
- Automated tests using xUnit

## Supported Payments

### Coins

- $0.10
- $0.20
- $0.50
- $1.00

### Notes

- $20.00
- $50.00

### Card

Card payments must match the exact remaining transaction amount.

## Project Structure

```text
VendingMachine/
├── ChangeDispenser.cs
├── Display.cs
├── Hardware.cs
├── Inventory.cs
├── KeypadPanel.cs
├── Payment.cs
├── Program.cs
├── SnackItem.cs
├── SnackMachine.cs
├── SnackSlot.cs
├── Transaction.cs
└── VendingMachine.csproj