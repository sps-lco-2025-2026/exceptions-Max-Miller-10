// Snippet A
int[] arr = new int[3];
arr[10] = 5;

// Snippet B
string s = null!;
Console.WriteLine(s.Length);

// Snippet C
int x = int.MaxValue;
checked { x = x + 1; }   // checked enforces overflow detection
