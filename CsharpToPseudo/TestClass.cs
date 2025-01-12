namespace CsharpToPseudo;

public class TestClass : IAnalyzable
{
    public string Lol = "";
    public string Lol2 { get; set; } = "asdads";

    public int SomeNumber = 1;

    public int SomeNumber2 { get; set; } = 2;

    public bool SomeBool = true;

    public bool SomeBool2 { get; set; } = false;

    public double SomeDouble = 1.1;

    public double SomeDouble2 { get; set; } = 2.2;

    public float SomeFloat = 1.1f;

    public float SomeFloat2 { get; set; } = 2.2f;

    public decimal SomeDecimal = 1.1m;

    public decimal SomeDecimal2 { get; set; } = 2.2m;

    public char SomeChar = 'a';

    public char SomeChar2 { get; set; } = 'b';

    public byte SomeByte = 1;

    public byte SomeByte2 { get; set; } = 2;

    public sbyte SomeSByte = 1;

    public sbyte SomeSByte2 { get; set; } = 2;

    public short SomeShort = 1;

    public short SomeShort2 { get; set; } = 2;

    public ushort SomeUShort = 1;

    public ushort SomeUShort2 { get; set; } = 2;

    public DateTime SomeDateTime { get; set; } = DateTime.UtcNow;

    public TimeOnly SomeTimeOnly { get; set; } = TimeOnly.MaxValue;
}