Console.Write("Enter your age: ");
string input = Console.ReadLine()!;

try
{
    int age = int.Parse(input);
    Console.WriteLine($"You are {age} years old.");
}
catch (FormatException e)
{
    Console.WriteLine($"That's not a valid number: {e.Message}");
}
