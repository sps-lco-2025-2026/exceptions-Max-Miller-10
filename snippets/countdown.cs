Console.Write("Count down from: ");

try
{
    int n = int.Parse(Console.ReadLine()!);
    for (int i = n; i >= 0; i--)
        Console.WriteLine(i);
}
catch (FormatException)
{
    Console.WriteLine("Please enter a whole number.");
}
finally
{
    Console.WriteLine("Countdown sequence ended.");  // always prints
}