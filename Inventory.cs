using System;

public class Inventory
{
	private readonly SnackSlot?[,] _snackSlot;

	public Inventory()
	{
		_snackSlot = new SnackSlot?[5, 5];
	}

	public SnackSlot GetSnackSlot(int row, int column)
	{
		if (row < 0 || row >= 5)
		{
			throw new ArgumentOutOfRangeException(nameof(row), "Invalid row.");
		}

		if (column < 0 || column >= 5)
		{
			throw new ArgumentOutOfRangeException(nameof(column), "Invalid column.");
		}

		SnackSlot? slot = _snackSlot[row, column];

		if (slot == null)
		{
			throw new InvalidSelectionException("The selected slot is empty.");
		}

		return slot;
	}

	public void AddSnackSlot(int row, int column, SnackSlot snackSlot)
	{
		if (row < 0 || row >= 5)
		{
			throw new ArgumentOutOfRangeException(nameof(row), "Invalid row.");
		}

		if (column < 0 || column >= 5)
		{
			throw new ArgumentOutOfRangeException(nameof(column), "Invalid column.");
		}

		_snackSlot[row, column] = snackSlot;
	}

	public SnackSlot GetSnackSlotByCode(string code)
	{
		for (int row = 0; row < 5; row++)
		{
			for (int column = 0; column < 5; column++)
			{
				SnackSlot? slot = _snackSlot[row, column];

				if (slot != null && slot.SlotCode == code)
				{
					return slot;
				}
			}
		}

		throw new InvalidSelectionException("Invalid keypad code.");
	}
}

public class InvalidSelectionException : Exception
{
	public InvalidSelectionException(string message) : base(message)
	{
	}
}