
void Greet(string name)
{
    if (name == "")
        throw new ArgumentException("Name cannot be empty.");
    Console.WriteLine($"Hello, {name}!");
}

void ProcessInput(string raw)
{
    string trimmed = raw.Trim();
    Greet(trimmed);           // exception can come from here
}

// --- program ---

Console.Write("Enter a name: ");
string input = Console.ReadLine()!;

try
{
    ProcessInput(input);      // caught here, two levels up
}
catch (ArgumentException e)
{
    Console.WriteLine($"Bad input: {e.Message}");
}