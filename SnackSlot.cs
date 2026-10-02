using System;

public class SnackSlot
{
	public string SlotCode { get; }
	public SnackItem Item { get; }
	public int StockCount { get; private set; }

	public SnackSlot(string slotCode, SnackItem snackItem, int stockCount)
	{
		SlotCode = slotCode;
		Item = snackItem;
		StockCount = stockCount;
	}

	public bool IsInStock()
	{
		return StockCount > 0;
	}

	public void DispenseSnack()
	{
		if (IsInStock())
		{
			StockCount--;
		}
		else
		{
			throw new OutOfStockException($"Snack item '{Item.Name}' is out of stock.");
		}
	}
}

public class OutOfStockException : Exception
{
	public OutOfStockException(string message) : base(message)
	{
	}
}