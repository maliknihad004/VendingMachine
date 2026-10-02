using System;

public interface IMachineState
{
	void SelectItem(SnackMachine machine, string code);
	void InsertPayment(SnackMachine machine, Payment payment);
	void CompletePurchase(SnackMachine machine);
	void Cancel(SnackMachine machine);
}

public class SnackMachine
{
	private IMachineState _currentState;
	private  Inventory _inventory;
	private  IPaymentProcessor _paymentProcessor;
	private  ChangeDispenser _changeDispenser;
	private  IDisplay _display;
	private  IKeypad _keypad;

	public Transaction? CurrentTransaction { get; private set; }

	public IMachineState CurrentState
	{
		get
		{
			return _currentState;
		}
	}

	public SnackMachine(IMachineState initialState, Inventory inventory, IPaymentProcessor paymentProcessor, ChangeDispenser changeDispenser, IDisplay display, IKeypad keypad)
	{
		_currentState = initialState;
		_inventory = inventory;
		_paymentProcessor = paymentProcessor;
		_changeDispenser = changeDispenser;
		_display = display;
		_keypad = keypad;
	}

	public void SetState(IMachineState state)
	{
		_currentState = state;
	}

	public SnackSlot GetSlot(string code)
	{
		return _inventory.GetSnackSlotByCode(code);
	}

	public void StartTransaction(SnackSlot slot)
	{
		CurrentTransaction = new Transaction(slot);
	}

	public void ValidatePayment(Payment payment)
	{
		if (CurrentTransaction == null)
		{
			throw new InvalidOperationException("No active transaction.");
		}

		decimal amountDue = CurrentTransaction.SelectedSlot.Item.Price - CurrentTransaction.Balance;

		_paymentProcessor.ValidatePayment(payment, amountDue);
	}

	public void AddPayment(Payment payment)
	{
		if (CurrentTransaction == null)
		{
			throw new InvalidOperationException("No active transaction.");
		}

		CurrentTransaction.AddPayment(payment);
		_changeDispenser.AddCash(payment);
	}

	public void ShowMessage(string message)
	{
		_display.ShowMessage(message);
	}

	public void SelectItem()
	{
		string code = _keypad.GetInput();
		_currentState.SelectItem(this, code);
	}

	public void InsertPayment(Payment payment)
	{
		_currentState.InsertPayment(this, payment);
	}

	public void CompletePurchase()
	{
		_currentState.CompletePurchase(this);
	}

	public void Cancel()
	{
		_currentState.Cancel(this);
	}

	public void CancelTransaction()
	{
		CurrentTransaction = null;
	}

	public void FinishTransaction()
	{
		if (CurrentTransaction == null)
		{
			throw new InvalidOperationException("No active transaction.");
		}

		CurrentTransaction.ValidateFunds();

		decimal changeAmount = CurrentTransaction.GetChange();

		if (!_changeDispenser.CanProvideChange(changeAmount))
		{
			throw new ChangeUnavailableException("Exact change is unavailable.");
		}

		CurrentTransaction.SelectedSlot.DispenseSnack();
		_changeDispenser.DispenseChange(changeAmount);

		_display.ShowMessage($"Change: ${changeAmount}");

		CurrentTransaction = null;
	}
}

public class IdleState : IMachineState
{
	public void SelectItem(SnackMachine machine, string code)
	{
		SnackSlot slot = machine.GetSlot(code);

		if (!slot.IsInStock())
		{
			throw new OutOfStockException("The selected snack is out of stock.");
		}

		machine.StartTransaction(slot);
		machine.ShowMessage($"Price: ${slot.Item.Price}");
		machine.SetState(new PaymentState());
	}

	public void InsertPayment(SnackMachine machine, Payment payment)
	{
		throw new InvalidOperationException("Select an item first.");
	}

	public void CompletePurchase(SnackMachine machine)
	{
		throw new InvalidOperationException("Select an item first.");
	}

	public void Cancel(SnackMachine machine)
	{
		throw new InvalidOperationException("No active transaction.");
	}
}

public class PaymentState : IMachineState
{
	public void SelectItem(SnackMachine machine, string code)
	{
		throw new InvalidOperationException("A transaction is already active.");
	}

	public void InsertPayment(SnackMachine machine, Payment payment)
	{
		machine.ValidatePayment(payment);
		machine.AddPayment(payment);

		if (machine.CurrentTransaction == null)
		{
			throw new InvalidOperationException("No active transaction.");
		}

		machine.ShowMessage($"Balance: ${machine.CurrentTransaction.Balance}");

		if (machine.CurrentTransaction.IsPaid())
		{
			machine.SetState(new DispensingState());
			machine.CompletePurchase();
		}
	}

	public void CompletePurchase(SnackMachine machine)
	{
		if (machine.CurrentTransaction == null)
		{
			throw new InvalidOperationException("No active transaction.");
		}

		machine.CurrentTransaction.ValidateFunds();

		machine.SetState(new DispensingState());
		machine.CompletePurchase();
	}

	public void Cancel(SnackMachine machine)
	{
		machine.CancelTransaction();
		machine.SetState(new IdleState());
		machine.ShowMessage("Transaction cancelled.");
	}
}

public class DispensingState : IMachineState
{
	public void SelectItem(SnackMachine machine, string code)
	{
		throw new InvalidOperationException("The machine is dispensing an item.");
	}

	public void InsertPayment(SnackMachine machine, Payment payment)
	{
		throw new InvalidOperationException("The machine is dispensing an item.");
	}

	public void CompletePurchase(SnackMachine machine)
	{
		try
		{
			machine.FinishTransaction();
			machine.SetState(new IdleState());
		}
		catch (ChangeUnavailableException)
		{
			machine.SetState(new PaymentState());
			throw;
		}
	}

	public void Cancel(SnackMachine machine)
	{
		throw new InvalidOperationException("The transaction cannot be cancelled while dispensing.");
	}
}