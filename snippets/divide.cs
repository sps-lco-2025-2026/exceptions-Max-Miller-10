Console.Write("Enter numerator: ");
int a = int.Parse(Console.ReadLine()!);

Console.Write("Enter denominator: ");
int b = int.Parse(Console.ReadLine()!);

try
{
    int result = a / b;
    Console.WriteLine($"Result: {result}");
}
catch (DivideByZeroException)
{
    Console.WriteLine("You can't divide by zero.");
}
catch (OverflowException e)
{
    Console.WriteLine($"The number was too large: {e.Message}");
}
catch (Exception e)
{
    Console.WriteLine($"Something unexpected went wrong: {e.Message}");
}