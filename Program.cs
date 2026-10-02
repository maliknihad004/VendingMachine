using System;

var inventory = new Inventory();

var chips = new SnackItem("Chips", 1.00m);
var chocolate = new SnackItem("Chocolate", 1.50m);
var cookies = new SnackItem("Cookies", 2.00m);
var crackers = new SnackItem("Crackers", 0.50m);
var candy = new SnackItem("Candy", 0.20m);

var chipsSlot = new SnackSlot("11", chips, 5);
var chocolateSlot = new SnackSlot("12", chocolate, 4);
var cookiesSlot = new SnackSlot("13", cookies, 3);
var crackersSlot = new SnackSlot("14", crackers, 6);
var candySlot = new SnackSlot("15", candy, 10);

inventory.AddSnackSlot(0, 0, chipsSlot);
inventory.AddSnackSlot(0, 1, chocolateSlot);
inventory.AddSnackSlot(0, 2, cookiesSlot);
inventory.AddSnackSlot(0, 3, crackersSlot);
inventory.AddSnackSlot(0, 4, candySlot);

var paymentProcessor = new PaymentProcessor();
var changeDispenser = new ChangeDispenser();

/*
	Initial machine cash:

	2 x $50   = $100
	2 x $20   = $40
	30 x $1   = $30
	20 x $0.50 = $10
	50 x $0.20 = $10
	100 x $0.10 = $10

	Total = $200
*/

for (int i = 0; i < 2; i++)
{
	changeDispenser.AddCash(new Payment("Note", 50.00m, "USD"));
}

for (int i = 0; i < 2; i++)
{
	changeDispenser.AddCash(new Payment("Note", 20.00m, "USD"));
}

for (int i = 0; i < 30; i++)
{
	changeDispenser.AddCash(new Payment("Coin", 1.00m, "USD"));
}

for (int i = 0; i < 20; i++)
{
	changeDispenser.AddCash(new Payment("Coin", 0.50m, "USD"));
}

for (int i = 0; i < 50; i++)
{
	changeDispenser.AddCash(new Payment("Coin", 0.20m, "USD"));
}

for (int i = 0; i < 100; i++)
{
	changeDispenser.AddCash(new Payment("Coin", 0.10m, "USD"));
}

var display = new Display();
var keypad = new KeypadPanel();

var machine = new SnackMachine(
	new IdleState(),
	inventory,
	paymentProcessor,
	changeDispenser,
	display,
	keypad
);

try
{
	display.ShowMessage("Enter item code:");

	machine.SelectItem();

	while (machine.CurrentTransaction != null)
	{
		display.ShowMessage("Enter payment type (Coin, Note, Card):");

		string? paymentType = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(paymentType))
		{
			throw new InvalidPaymentException("Invalid payment type.");
		}

		display.ShowMessage("Enter payment amount:");

		string? amountInput = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(amountInput))
		{
			throw new InvalidPaymentException("Invalid payment amount.");
		}

		decimal amount;

		try
		{
			amount = decimal.Parse(amountInput);
		}
		catch (FormatException)
		{
			throw new InvalidPaymentException("Invalid payment amount.");
		}

		var payment = new Payment(paymentType, amount, "USD");

		machine.InsertPayment(payment);
	}
}
catch (Exception ex)
{
	display.ShowMessage(ex.Message);
}

Console.WriteLine();
Console.WriteLine("Remaining stock:");
Console.WriteLine($"Chips: {chipsSlot.StockCount}");
Console.WriteLine($"Chocolate: {chocolateSlot.StockCount}");
Console.WriteLine($"Cookies: {cookiesSlot.StockCount}");
Console.WriteLine($"Crackers: {crackersSlot.StockCount}");
Console.WriteLine($"Candy: {candySlot.StockCount}");