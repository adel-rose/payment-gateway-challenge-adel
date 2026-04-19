namespace PaymentGateway.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class BankCommunicationException : Exception
{
    public BankCommunicationException(string message) : base(message)
    {
    }
}