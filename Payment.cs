using System;

public class Payment
{
	public string PaymentType { get; }
	public decimal Amount { get; }
	public string Currency { get; }

	public Payment(string paymentType, decimal amount, string currency)
	{
		PaymentType = paymentType;
		Amount = amount;
		Currency = currency;
	}
}

public interface IPaymentProcessor
{
	void ValidatePayment(Payment payment, decimal amountDue);
}

public class PaymentProcessor : IPaymentProcessor
{
	public void ValidatePayment(Payment payment, decimal amountDue)
	{
		if (payment.Currency != "USD")
		{
			throw new InvalidPaymentException("Only USD currency is accepted.");
		}

		if (payment.PaymentType == "Coin")
		{
			if (payment.Amount != 0.10m &&
				payment.Amount != 0.20m &&
				payment.Amount != 0.50m &&
				payment.Amount != 1.00m)
			{
				throw new InvalidPaymentException("Unsupported coin denomination.");
			}
		}

		if (payment.PaymentType == "Note")
		{
			if (payment.Amount != 20.00m &&
				payment.Amount != 50.00m)
			{
				throw new InvalidPaymentException("Unsupported note denomination.");
			}
		}

		if (payment.PaymentType == "Card")
		{
			if (payment.Amount != amountDue)
			{
				throw new InvalidPaymentException("Card payment must match the exact amount due.");
			}
		}

		if (payment.PaymentType != "Coin" &&
			payment.PaymentType != "Note" &&
			payment.PaymentType != "Card")
		{
			throw new InvalidPaymentException("Unsupported payment type.");
		}
	}
}

public class InvalidPaymentException : Exception
{
	public InvalidPaymentException(string message) : base(message)
	{
	}
}