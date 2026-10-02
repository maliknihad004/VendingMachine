public interface IDisplay
{
	void ShowMessage(string message);
}

public interface IKeypad
{
	string GetInput();
}