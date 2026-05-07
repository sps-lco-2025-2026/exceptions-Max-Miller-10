
// Base class for all banking domain errors
class BankException : Exception
{
    public BankException(string message) : base(message) { }
    public BankException(string message, Exception inner) : base(message, inner) { }
}

// More specific subclasses
class InsufficientFundsException : BankException
{
    public decimal Balance { get; }
    public decimal Amount { get; }

    public InsufficientFundsException(decimal balance, decimal amount)
        : base($"Cannot withdraw {amount:C} - balance is only {balance:C}.")
    {
        Balance = balance;
        Amount = amount;
    }
}

class AccountFrozenException : BankException
{
    public AccountFrozenException()
        : base("This account has been frozen and cannot be used.") { }
}

// --- program ---

decimal balance = 50.00m;
bool frozen = false;

Console.Write("Enter amount to withdraw: ");

try
{
    decimal amount = decimal.Parse(Console.ReadLine()!);

    if (frozen)
        throw new AccountFrozenException();

    if (amount > balance)
        throw new InsufficientFundsException(balance, amount);

    balance -= amount;
    Console.WriteLine($"Withdrawn. New balance: {balance:C}");
}
catch (InsufficientFundsException e)
{
    // Catches only this specific subtype
    Console.WriteLine(e.Message);
    Console.WriteLine($"You are {e.Amount - e.Balance:C} short.");
}
catch (BankException e)
{
    // Catches AccountFrozenException and any other BankException
    Console.WriteLine($"Banking error: {e.Message}");
}
catch (FormatException)
{
    Console.WriteLine("Please enter a valid amount.");
}