using System;

public class KeypadPanel : IKeypad
{
	public string GetInput()
	{
		string? input = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(input))
		{
			throw new InvalidSelectionException("Invalid keypad code.");
		}

		try
		{
			int numericCode = int.Parse(input);
		}
		catch (FormatException)
		{
			throw new InvalidSelectionException("Invalid keypad code.");
		}

		return input;
	}
}