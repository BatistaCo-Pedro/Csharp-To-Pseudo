namespace CsharpToPseudo;

public class TestClass2 : AbstractTest, ITestInterface
{
    // Fields
    public int publicField;
    private string privateField = "PrivateValue";
    protected static double staticField = 3.14;

    // Constructors
    public TestClass2()
    {
        publicField = 42;
    }

    public TestClass2(int value, string text)
    {
        publicField = value;
        privateField = text;
    }

    // Properties
    public int AutoProperty { get; set; } // Auto-implemented property
    public string ReadOnlyProperty { get; } = "ReadOnly"; // Read-only auto-property with initializer
    public string ComputedProperty // Computed (getter-only) property
    {
        get { return privateField.ToUpper(); }
    }
    public static string StaticProperty { get; set; } = "Static";

    // Indexer
    private int[] _data = new int[] { 1, 2, 3, 4, 5 };
    public int this[int index]
    {
        get { return _data[index]; }
        set { _data[index] = value; }
    }

    // Methods

    // A simple instance method.
    public void DoSomething()
    {
        Console.WriteLine("Doing something...");
    }

    // A static method.
    public static void StaticMethod()
    {
        Console.WriteLine("Static method called.");
    }

    // Override the abstract method.
    public override void AbstractMethod()
    {
        Console.WriteLine("Implemented abstract method.");
    }

    // Override a virtual method.
    public override void VirtualMethod()
    {
        Console.WriteLine("Overridden virtual method.");
    }

    // Implement the interface.
    public void InterfaceMethod()
    {
        Console.WriteLine("Interface method implemented.");
    }

    // Event declaration.
    public event EventHandler TestEvent;

    protected virtual void OnTestEvent()
    {
        TestEvent?.Invoke(this, EventArgs.Empty);
    }

    // Operator overload (adds two TestClass instances by combining their fields).
    public static TestClass2 operator +(TestClass2 a, TestClass2 b)
    {
        return new TestClass2(a.publicField + b.publicField, a.privateField + b.privateField);
    }

    // Finalizer (destructor).
    ~TestClass2()
    {
        Console.WriteLine("Finalizing TestClass instance.");
    }

    // Nested type.
    public class NestedClass
    {
        public void NestedMethod()
        {
            Console.WriteLine("Nested method called.");
        }
    }
}