namespace CSharpToPseudo.Benchmarks;

public struct TestStruct : IAnalyzable
{
    // Fields
    public int X;
    public int Y;

    // Constructor
    public TestStruct(int x, int y)
    {
        X = x;
        Y = y;
    }

    // A computed property.
    public int Sum
    {
        get { return X + Y; }
    }

    // A static property in a struct.
    public static int StaticValue { get; set; } = 100;

    // A method.
    public void DoStructWork()
    {
        Console.WriteLine($"Struct work: X={X}, Y={Y}");
    }

    // Operator overload.
    public static TestStruct operator +(TestStruct a, TestStruct b)
    {
        return new TestStruct(a.X + b.X, a.Y + b.Y);
    }
}