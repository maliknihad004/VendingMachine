using Xunit;

public class SnackMachineTests
{
	[Fact]
	public void Purchase_ExactPayment_DispensesSnackAndReturnsToIdle()
	{
		var snack = new SnackItem("Chips", 1.00m);
		var slot = new SnackSlot("11", snack, 5);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var paymentProcessor = new PaymentProcessor();
		var changeDispenser = new ChangeDispenser();
		var display = new TestDisplay();
		var keypad = new TestKeypad("11");

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			paymentProcessor,
			changeDispenser,
			display,
			keypad
		);

		var payment = new Payment("Coin", 1.00m, "USD");

		machine.SelectItem();
		machine.InsertPayment(payment);

		Assert.Equal(4, slot.StockCount);
		Assert.Null(machine.CurrentTransaction);
		Assert.IsType<IdleState>(machine.CurrentState);
	}

	[Fact]
	public void Purchase_WithChange_DispensesSnackAndChange()
	{
		var snack = new SnackItem("Crackers", 0.50m);
		var slot = new SnackSlot("14", snack, 5);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var paymentProcessor = new PaymentProcessor();
		var changeDispenser = new ChangeDispenser();

		changeDispenser.AddCash(new Payment("Coin", 0.50m, "USD"));

		var display = new TestDisplay();
		var keypad = new TestKeypad("14");

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			paymentProcessor,
			changeDispenser,
			display,
			keypad
		);

		var payment = new Payment("Coin", 1.00m, "USD");

		machine.SelectItem();
		machine.InsertPayment(payment);

		Assert.Equal(4, slot.StockCount);
		Assert.Null(machine.CurrentTransaction);
		Assert.IsType<IdleState>(machine.CurrentState);
		Assert.False(changeDispenser.CanProvideChange(0.50m));
	}

	[Fact]
	public void SelectItem_OutOfStock_ThrowsOutOfStockException()
	{
		var snack = new SnackItem("Chips", 1.00m);
		var slot = new SnackSlot("11", snack, 0);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			new PaymentProcessor(),
			new ChangeDispenser(),
			new TestDisplay(),
			new TestKeypad("11")
		);

		Assert.Throws<OutOfStockException>(() => machine.SelectItem());

		Assert.Equal(0, slot.StockCount);
	}

	[Fact]
	public void CompletePurchase_InsufficientBalance_ThrowsInsufficientFundsException()
	{
		var snack = new SnackItem("Chips", 1.00m);
		var slot = new SnackSlot("11", snack, 5);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			new PaymentProcessor(),
			new ChangeDispenser(),
			new TestDisplay(),
			new TestKeypad("11")
		);

		var payment = new Payment("Coin", 0.50m, "USD");

		machine.SelectItem();
		machine.InsertPayment(payment);

		Assert.Throws<InsufficientFundsException>(() => machine.CompletePurchase());

		Assert.Equal(5, slot.StockCount);
		Assert.IsType<PaymentState>(machine.CurrentState);
	}

	[Fact]
	public void SelectItem_InvalidCode_ThrowsInvalidSelectionException()
	{
		var inventory = new Inventory();

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			new PaymentProcessor(),
			new ChangeDispenser(),
			new TestDisplay(),
			new TestKeypad("99")
		);

		Assert.Throws<InvalidSelectionException>(() => machine.SelectItem());
	}

	[Fact]
	public void InsertPayment_UnsupportedCoin_ThrowsInvalidPaymentException()
	{
		var snack = new SnackItem("Chips", 1.00m);
		var slot = new SnackSlot("11", snack, 5);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			new PaymentProcessor(),
			new ChangeDispenser(),
			new TestDisplay(),
			new TestKeypad("11")
		);

		machine.SelectItem();

		var payment = new Payment("Coin", 0.30m, "USD");

		Assert.Throws<InvalidPaymentException>(() => machine.InsertPayment(payment));

		Assert.Equal(5, slot.StockCount);
	}

	[Fact]
	public void InsertPayment_UnsupportedNote_ThrowsInvalidPaymentException()
	{
		var snack = new SnackItem("Chips", 1.00m);
		var slot = new SnackSlot("11", snack, 5);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			new PaymentProcessor(),
			new ChangeDispenser(),
			new TestDisplay(),
			new TestKeypad("11")
		);

		machine.SelectItem();

		var payment = new Payment("Note", 10.00m, "USD");

		Assert.Throws<InvalidPaymentException>(() => machine.InsertPayment(payment));
	}

	[Fact]
	public void InsertPayment_CardAmountNotExact_ThrowsInvalidPaymentException()
	{
		var snack = new SnackItem("Candy", 0.20m);
		var slot = new SnackSlot("15", snack, 10);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			new PaymentProcessor(),
			new ChangeDispenser(),
			new TestDisplay(),
			new TestKeypad("15")
		);

		machine.SelectItem();

		var payment = new Payment("Card", 1.00m, "USD");

		Assert.Throws<InvalidPaymentException>(() => machine.InsertPayment(payment));

		Assert.Equal(10, slot.StockCount);
	}

	[Fact]
	public void Cancel_ActiveTransaction_ReturnsToIdleState()
	{
		var snack = new SnackItem("Chocolate", 1.50m);
		var slot = new SnackSlot("12", snack, 4);

		var inventory = new Inventory();
		inventory.AddSnackSlot(0, 0, slot);

		var machine = new SnackMachine(
			new IdleState(),
			inventory,
			new PaymentProcessor(),
			new ChangeDispenser(),
			new TestDisplay(),
			new TestKeypad("12")
		);

		machine.SelectItem();

		machine.Cancel();

		Assert.Null(machine.CurrentTransaction);
		Assert.IsType<IdleState>(machine.CurrentState);
		Assert.Equal(4, slot.StockCount);
	}
}

public class TestDisplay : IDisplay
{
	public void ShowMessage(string message)
	{
	}
}

public class TestKeypad : IKeypad
{
	private readonly string _input;

	public TestKeypad(string input)
	{
		_input = input;
	}

	public string GetInput()
	{
		return _input;
	}
}