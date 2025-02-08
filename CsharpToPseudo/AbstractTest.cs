namespace CsharpToPseudo;

public abstract class AbstractTest : IAnalyzable
{
    public abstract void AbstractMethod();

    public virtual void VirtualMethod()
    {
        Console.WriteLine("Default VirtualMethod in AbstractTest");
    }
}