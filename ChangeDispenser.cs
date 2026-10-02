using System;
using System.Collections.Generic;

public class ChangeDispenser
{
	private readonly Stack<decimal> _coinStack;
	private readonly Stack<decimal> _noteStack;

	public ChangeDispenser()
	{
		_coinStack = new Stack<decimal>();
		_noteStack = new Stack<decimal>();
	}

	public void AddCash(Payment payment)
	{
		if (payment.PaymentType == "Coin")
		{
			_coinStack.Push(payment.Amount);
		}

		if (payment.PaymentType == "Note")
		{
			_noteStack.Push(payment.Amount);
		}
	}

	public bool CanProvideChange(decimal changeAmount)
	{
		if (changeAmount == 0)
		{
			return true;
		}

		var availableCash = new List<decimal>();

		foreach (decimal coin in _coinStack)
		{
			availableCash.Add(coin);
		}

		foreach (decimal note in _noteStack)
		{
			availableCash.Add(note);
		}

		availableCash.Sort();
		availableCash.Reverse();

		var cashToDispense = new List<decimal>();

		if (CanMakeChangeGreedy(availableCash, changeAmount, cashToDispense))
		{
			return true;
		}

		cashToDispense.Clear();

		return CanMakeExactChange(availableCash, changeAmount, 0, cashToDispense);
	}

	private bool CanMakeChangeGreedy(List<decimal> availableCash, decimal changeAmount, List<decimal> cashToDispense)
	{
		decimal remainingAmount = changeAmount;

		foreach (decimal cash in availableCash)
		{
			if (cash <= remainingAmount)
			{
				remainingAmount -= cash;
				cashToDispense.Add(cash);
			}

			if (remainingAmount == 0)
			{
				return true;
			}
		}

		return false;
	}

	private bool CanMakeExactChange(List<decimal> availableCash, decimal remainingAmount, int index, List<decimal> cashToDispense)
	{
		if (remainingAmount == 0)
		{
			return true;
		}

		if (remainingAmount < 0 || index >= availableCash.Count)
		{
			return false;
		}

		cashToDispense.Add(availableCash[index]);

		if (CanMakeExactChange(availableCash, remainingAmount - availableCash[index], index + 1, cashToDispense))
		{
			return true;
		}

		cashToDispense.RemoveAt(cashToDispense.Count - 1);

		return CanMakeExactChange(availableCash, remainingAmount, index + 1, cashToDispense);
	}

	public void DispenseChange(decimal changeAmount)
	{
		if (changeAmount == 0)
		{
			return;
		}

		var availableCash = new List<decimal>();

		foreach (decimal coin in _coinStack)
		{
			availableCash.Add(coin);
		}

		foreach (decimal note in _noteStack)
		{
			availableCash.Add(note);
		}

		availableCash.Sort();
		availableCash.Reverse();

		var cashToDispense = new List<decimal>();

		if (!CanMakeChangeGreedy(availableCash, changeAmount, cashToDispense))
		{
			cashToDispense.Clear();

			if (!CanMakeExactChange(availableCash, changeAmount, 0, cashToDispense))
			{
				throw new ChangeUnavailableException("Exact change is unavailable.");
			}
		}

		foreach (decimal cash in cashToDispense)
		{
			if (cash == 20.00m || cash == 50.00m)
			{
				RemoveCash(_noteStack, cash);
			}
			else
			{
				RemoveCash(_coinStack, cash);
			}
		}
	}

	private void RemoveCash(Stack<decimal> stack, decimal amount)
	{
		var temporaryStack = new Stack<decimal>();

		while (stack.Count > 0)
		{
			decimal cash = stack.Pop();

			if (cash == amount)
			{
				break;
			}

			temporaryStack.Push(cash);
		}

		while (temporaryStack.Count > 0)
		{
			stack.Push(temporaryStack.Pop());
		}
	}
}

public class ChangeUnavailableException : Exception
{
	public ChangeUnavailableException(string message) : base(message)
	{
	}
}