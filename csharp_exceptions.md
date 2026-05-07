# C# Exceptions — Teaching Notes & Exercises

### .NET 10 · single-file apps · `dotnet run file.cs`

When you've cloned the repo, in VS Code you should just be able to click the play button on each .cs file and run it independently, with no extra .csproj file. Alternatively, open the terminal `exceptions` directory and just use `dotnet` directly (optionally with `build` or `run` in between `dotnet` and the filename, with the directory).

```bash
dotnet run snippets/greet.cs
dotnet exercises/1.crash.cs
```



---

## Part 1: `try` / `catch` / `finally`

### What is an exception?

When something goes wrong at runtime, C# **throws an exception** — an object that describes the error. You already have some awareness of classes and inheritance: every exception is an instance of a class that inherits (directly or indirectly) from `System.Exception`. *If nothing catches it*, the program terminates with an error message.

#### `If nothing catches it' 

The exception travels up the call stack; if the current function fails, it gets passed up the stack to the calling functions, unless or until it gets dealt with. We will come back to this later.

---

### Basic `try`/`catch`

```csharp
// greet.cs
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
```

Run with: `dotnet run greet.cs` (or with `snippets/` if you're in the top-level repo folder).

Key points:
- The `try` block runs normally until something throws.
- The `catch` block only runs if an exception of the **matching type** (or a subtype) is thrown.
- `e` gives access to the exception object, which has useful properties that you can print out or log: `e.Message`, `e.GetType().Name`, `e.StackTrace`.

---

### Multiple `catch` blocks

Stack them top to bottom — C# runs the **first match** it finds (think like Python with `if ... elif ... else ...`).

```csharp
// divide.cs
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
```

> **Rule:** Put more specific types first, more general types last.  
> A `catch (Exception)` at the top would swallow everything — legal C# but poor practice (`lawful but awful').

---

### The `finally` block

`finally` runs **no matter what** — exception thrown or not. It's the right place for any cleanup that must always happen.

```csharp
// countdown.cs
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
```

---

### Useful built-in exception types

| Exception | Common cause |
|---|---|
| `FormatException` | `int.Parse("abc")` |
| `DivideByZeroException` | `x / 0` with integers |
| `IndexOutOfRangeException` | Accessing `arr[99]` when array is shorter |
| `NullReferenceException` | Calling a method on a `null` object |
| `OverflowException` | Arithmetic result too large for the type |
| `ArgumentException` | Invalid argument passed to a method |
| `ArgumentNullException` | `null` passed where it isn't allowed (subtype of `ArgumentException`) |

---

## Part 1: Exercises

### Exercise 1 — Catch the crash

Run this deliberately broken code and note the unhandled exception:

```csharp
// crash.cs
string[] names = { "Alice", "Bob", "Charlie" };
Console.Write("Enter an index: ");
int i = int.Parse(Console.ReadLine()!);
Console.WriteLine(names[i]);
```

**Task:** Wrap it in a `try`/`catch` that handles:
1. A non-numeric input (`FormatException`)
2. An index that's out of range (`IndexOutOfRangeException`)

Print a clear, user-friendly message for each case.

---

### Exercise 2 — Multiple catches

```csharp
// calc.cs — starter
Console.Write("Enter a number: ");
int n = int.Parse(Console.ReadLine()!);
Console.WriteLine(100 / n);
```

**Task:** Add `catch` blocks for at least two different exception types that could be thrown here. Test each path by entering a bad value, then zero, then a valid number. Add a `finally` block that always prints `"Calculation complete."`.

**Discussion question:** Could both exceptions be thrown on the same run? Why or why not?

---

### Exercise 3 — `finally` in action

Write a program that:
1. Asks the user for a number
2. Prints whether it's even or odd
3. Has a `finally` block that always prints `"Thank you for using the program."`

Test three scenarios — valid even, valid odd, non-numeric input — and confirm the `finally` message appears in all three.

---

### Exercise 4 — Exception investigation

For each snippet and `catch` the appropriate exception type:

```csharp
// Snippet A
int[] arr = new int[3];
arr[10] = 5;

// Snippet B
string s = null!;
Console.WriteLine(s.Length);

// Snippet C
int x = int.MaxValue;
checked { x = x + 1; }   // checked enforces overflow detection
```

---

## Part 1.5: Exceptions Across Function Calls

### How exceptions propagate

When an exception is thrown inside a function and not caught there, it travels **up the call stack** until something catches it — or the program terminates. Students who understand the call stack will find this natural; the exception just keeps unwinding frames until it finds a matching `catch`.

```csharp
// stack.cs

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
```

`Greet` throws, `ProcessInput` doesn't catch it, so it arrives at the `try`/`catch` in the top-level code. The stack trace on `e` shows the full journey.

---

### Catching in the function vs letting it propagate

Sometimes the right place to handle an exception is inside the function that caused it. Other times the function can't know what to do — it should let the caller decide. There's no universal rule; it's a design decision.

```csharp
// parse.cs

// Option A: handle it internally, return a fallback
int SafeParse(string s, int fallback)
{
    try
    {
        return int.Parse(s);
    }
    catch (FormatException)
    {
        return fallback;
    }
}

// Option B: let it propagate — the caller handles it
int StrictParse(string s)
{
    return int.Parse(s);   // FormatException travels up if thrown
}
```

A good question for discussion: which design is better? It depends on whether a bad value is a *recoverable* situation (Option A) or a *genuine error* the caller needs to know about (Option B).

---

### `throw` vs `throw ex` vs `throw new Exception(...)`

Once you've caught an exception, you have three options for re-raising it. They behave differently and the difference matters.

#### `throw` — re-throw, preserve the stack trace

```csharp
// rethrow.cs

void Validate(int n)
{
    try
    {
        if (n < 0) throw new ArgumentException("Must be non-negative.");
    }
    catch (ArgumentException)
    {
        Console.WriteLine("Validation failed — logging the error.");
        throw;   // re-throws the original exception, stack trace intact
    }
}

try
{
    Validate(-1);
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine(e.StackTrace);  // still points at the throw inside Validate
}
```

Use `throw` (bare) when you want to do something — log, clean up — but still let the original exception bubble up unchanged. **The stack trace is preserved**, so the caller can see exactly where the problem originated.

---

#### `throw ex` — re-throws, but destroys the stack trace

```csharp
catch (ArgumentException ex)
{
    Console.WriteLine("Logging...");
    throw ex;   // re-throws, but stack trace now starts HERE
}
```

`throw ex` resets the stack trace to the current line, losing the original origin. This makes debugging much harder. **Avoid `throw ex`** — there is almost _never_ a good reason to use it. It exists as a language feature but is widely considered a mistake to use. I've included it here so you know it you see it. 

**Aside:** there is possibly one time you might excusable see it ... 

---

#### `throw new Exception(...)` — wrap in a new exception

Sometimes you want to translate a low-level exception into a higher-level one that makes more sense to the caller. Wrap the original as the `InnerException` so the full chain is preserved:

```csharp
// wrap.cs

int ParseAge(string s)
{
    try
    {
        return int.Parse(s);
    }
    catch (FormatException ex)
    {
        // Translate: FormatException is a detail the caller shouldn't need to know about
        throw new ArgumentException($"'{s}' is not a valid age.", ex);
        //                                                         ^^ inner exception
    }
}

try
{
    int age = ParseAge("twenty");
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine($"Caused by: {e.InnerException?.Message}");
}
```

The `InnerException` chain means no information is lost — a debugger or logging framework can still see the original `FormatException`. This is what the `(string message, Exception inner)` constructor is for.

---

### The three options at a glance

| | Preserves original stack trace | Preserves original exception type | Use when |
|---|:---:|:---:|---|
| `throw` | ✅ | ✅ | You need to do something (log/clean up) then let it continue |
| `throw ex` | ❌ | ✅ | _Almost_ never — avoid |
| `throw new X(..., ex)` | ✅ (via InnerException) | ❌ (intentionally) | Translating to a more meaningful exception for the caller |

---

### Exercise 5 — Propagation and rethrowing

Given this program structure:

```csharp
// chain.cs

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
```

**Tasks:**
1. Run it and enter `0` as the denominator. Note which function threw and where it was caught (nowhere — it crashed).
2. Add a `try`/`catch` inside `Divide` that logs `"Division attempted"` then uses bare `throw` to re-raise. Confirm the stack trace in the caller still points at `Divide`.
3. Change it to `throw ex` and compare the stack trace. What's different?
4. Change `Divide` to catch `DivideByZeroException` and throw a new `ArgumentException("Denominator cannot be zero.", ex)` instead. Catch this in the top-level code and print both the message and the `InnerException` message.

---

## Part 2: Custom Exception Classes

### Why create your own?

Built-in exceptions describe *technical* failures. Custom exceptions let you describe *domain* failures in your own language — things the built-in types can't express.

Consider a program that manages quiz scores. `ArgumentException` could cover a score of -5, but `InvalidScoreException` tells the next developer (or your future self) exactly what went wrong.

---

### Defining a custom exception

Inherit from `Exception` and provide at least the standard constructors:

```csharp
// quiz.cs

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
```

Points to highlight:
- `throw new InvalidScoreException(...)` works exactly like throwing any built-in exception.
- The three-constructor *pattern* is a C# convention — always include the `inner` one so exceptions can be chained.
- For this excercise the class goes in the same `.cs` file above the top-level statements; no project structure needed.

---

### Adding extra information

Because it's a class, you can add properties that carry extra context:

```csharp
// quiz2.cs

class InvalidScoreException : Exception
{
    public int AttemptedScore { get; }

    public InvalidScoreException(int score)
        : base($"{score} is not valid - must be between 0 and 100.")
    {
        AttemptedScore = score;
    }

    public InvalidScoreException(string message, Exception inner)
        : base(message, inner) { }
}

// --- program ---

Console.Write("Enter score: ");

try
{
    int score = int.Parse(Console.ReadLine()!);

    if (score < 0 || score > 100)
        throw new InvalidScoreException(score);

    Console.WriteLine("Score recorded.");
}
catch (InvalidScoreException e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine($"(You entered: {e.AttemptedScore})");
}
catch (FormatException)
{
    Console.WriteLine("That wasn't a number at all.");
}
```

---

### Inheriting from a more specific base

You can inherit from any exception class, not just `Exception`. This lets callers catch at different levels of specificity — the same inheritance rules they already know.

```csharp
// bank.cs

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
```

The catch order mirrors the hierarchy: specific subclasses first, then the base class, then built-ins. This is the same rule as Part 1 — a derived type IS the base type, so the base catch would match first if placed earlier.

---

## Part 2: Exercises

### Exercise 6.1 — Your first custom exception

Write a temperature converter (Celsius to Fahrenheit). Define a `TemperatureException` that is thrown when the input is below absolute zero (−273.15 °C). Catch it separately from a `FormatException` (bad input).

Starter structure:

```csharp
// temp.cs

class TemperatureException : Exception
{
    // your constructors here
}

// program below...
```

---

### Exercise 6.2 — Extra properties

Extend `TemperatureException` to carry an `AttemptedValue` property (the `double` that was rejected). In the catch block, use this property to print a message like:

> `-300 is below absolute zero (minimum: -273.15)`

---

### Exercise 7 — An exception hierarchy

Model a simple game inventory system. Create:

- `InventoryException : Exception` — base class for all inventory errors
- `ItemNotFoundException : InventoryException` — item name not in the inventory
- `InsufficientQuantityException : InventoryException` — not enough of an item

Write a program that holds a small `Dictionary<string, int>` of items and counts. Let the user type commands like `take sword` or `take potion`. Throw the appropriate exception if the item doesn't exist or the count would go below zero. Catch each type separately with a meaningful message.

---

### Exercise 8 — Discussion questions

1. Why should `InsufficientQuantityException` inherit from `InventoryException` rather than directly from `Exception`?
1. Could a caller catch both `ItemNotFoundException` and `InsufficientQuantityException` with a single `catch` block? How?
1. When would you choose to catch `Exception` rather than a specific type?
1. What's the purpose of the `(string message, Exception inner)` constructor? When would you use `inner`?
1. Discuss when you might explicitly deploy the `throw ex` pattern. 

---

## Summary

| Concept | One-liner |
|---|---|
| `try` | Wraps code that might throw |
| `catch (SomeException e)` | Runs if that type or a subtype is thrown |
| Multiple `catch` blocks | Most specific first, most general last |
| `finally` | Always runs — cleanup that must not be skipped |
| `throw new X(...)` | You can throw exceptions yourself |
| `class MyEx : Exception` | Custom exception — inherits everything |
| Extra properties | Add context a built-in type can't carry |
| Exception hierarchies | Same inheritance rules as any other class |
