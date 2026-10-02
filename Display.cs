using System;

public class Display : IDisplay
{
	public void ShowMessage(string message)
	{
		Console.WriteLine(message);
	}
}