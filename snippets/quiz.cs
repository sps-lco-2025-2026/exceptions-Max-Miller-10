
class InvalidScoreException : Exception
{
    public InvalidScoreException() { }

    public InvalidScoreException(string message)
        : base(message) { }

    public InvalidScoreException(string message, Exception inner)
        : base(message, inner) { }
}

// --- program starts here ---

Console.Write("Enter score (0-100): ");
int score = int.Parse(Console.ReadLine()!);

try
{
    if (score < 0 || score > 100)
        throw new InvalidScoreException($"{score} is not a valid score.");

    Console.WriteLine($"Score accepted: {score}");
}
catch (InvalidScoreException e)
{
    Console.WriteLine($"Domain error: {e.Message}");
}