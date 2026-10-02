using System;

public class Transaction
{
	public SnackSlot SelectedSlot { get; }
	public decimal Balance { get; private set; }

	public Transaction(SnackSlot selectedSlot)
	{
		SelectedSlot = selectedSlot;
		Balance = 0;
	}

	public void AddPayment(Payment payment)
	{
		Balance += payment.Amount;
	}

	public bool IsPaid()
	{
		return Balance >= SelectedSlot.Item.Price;
	}

	public decimal GetChange()
	{
		return Balance - SelectedSlot.Item.Price;
	}

	public void ValidateFunds()
	{
		if (!IsPaid())
		{
			throw new InsufficientFundsException("Insufficient funds.");
		}
	}
}

public class InsufficientFundsException : Exception
{
	public InsufficientFundsException(string message) : base(message)
	{
	}
}