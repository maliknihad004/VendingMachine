public class SnackItem
{
	public string Name { get; }
	public decimal Price { get; }

	public SnackItem(string name, decimal price)
	{
		Name = name;
		Price = price;
	}
}