
int Divide(int a, int b)
{
    return a / b;
}

int ReadAndDivide()
{
    Console.Write("Numerator: ");
    int a = int.Parse(Console.ReadLine()!);
    Console.Write("Denominator: ");
    int b = int.Parse(Console.ReadLine()!);
    return Divide(a, b);
}

Console.WriteLine(ReadAndDivide());