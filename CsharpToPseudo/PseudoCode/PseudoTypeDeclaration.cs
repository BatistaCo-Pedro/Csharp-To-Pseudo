using System.Collections.Immutable;
using System.Text;

namespace CsharpToPseudo.PseudoCode;

public record PseudoTypeDeclaration
{
    public NonEmptyString Name { get; init; }
    
    public NonEmptyString Type { get; init; }
    
    public ImmutableList<PseudoModifier> Modifiers { get; init; }

    public ImmutableList<PseudoProperty> Properties { get; init; }
    public ImmutableList<PseudoField> Fields { get; init; }
    public ImmutableList<PseudoMethod> Methods { get; init; }

    public PseudoTypeDeclaration(
        NonEmptyString name,
        NonEmptyString type,
        IEnumerable<PseudoModifier> modifiers,
        IEnumerable<PseudoProperty> properties,
        IEnumerable<PseudoField> fields,
        IEnumerable<PseudoMethod> methods
    )
    {
        Name = name;
        Type = type;
        Modifiers = modifiers.ToImmutableList();
        Properties = properties.ToImmutableList();
        Fields = fields.ToImmutableList();
        Methods = methods.ToImmutableList();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(PseudoModifier.Combine(Modifiers));
        sb.Append(Type);
        sb.Append(' ');
        sb.Append(Name);
        sb.Append(' ');
        sb.Append('{');
        sb.Append('\n');
        // set fields, properties, methods etc..
        
        return sb.ToString();
    }
}

public abstract record PseudoMember
{
    public NonEmptyString Name { get; init; }

    public ImmutableList<PseudoModifier> Modifiers { get; init; }

    public PseudoType Type { get; init; }
    
    public string? Value { get; init; }

    public PseudoMember(NonEmptyString name, IEnumerable<PseudoModifier> modifiers, PseudoType type, string? value)
    {
        Name = name;
        Modifiers = modifiers.ToImmutableList();
        Type = type;
        Value = value;
    }
}

public record PseudoProperty : PseudoMember
{
    public PseudoPropertyMethod Getter { get; init; }
    public PseudoPropertyMethod? Setter { get; init; }

    public PseudoProperty(
        NonEmptyString name,
        IEnumerable<PseudoModifier> modifiers,
        PseudoType type,
        PseudoPropertyMethod getter,
        PseudoPropertyMethod? setter,
        string? value = null
    )
        : base(name, modifiers, type, value)
    {
        Getter = getter;
        Setter = setter;
    }
}

public record PseudoPropertyMethod
{
    public PseudoModifier Modifier { get; init; }

    public PropertyMethodType MethodType { get; private set; }

    public PseudoPropertyMethod(PseudoModifier modifier, PropertyMethodType methodType)
    {
        Modifier = modifier;
        MethodType = methodType;
    }
}

public record PseudoConstructor : PseudoMethod
{
    public PseudoConstructor(
        NonEmptyString name,
        IEnumerable<PseudoModifier> modifiers,
        IEnumerable<PseudoParameter> parameters
    )
        : base(name, modifiers, null, parameters) { }
}

public record PseudoMethod
{
    public NonEmptyString Name { get; init; }

    public ImmutableList<PseudoModifier> Modifiers { get; init; }

    public PseudoType? ReturnType { get; init; }

    public ImmutableList<PseudoParameter> Parameters { get; init; }

    public PseudoMethod(
        NonEmptyString name,
        IEnumerable<PseudoModifier> modifiers,
        PseudoType? returnType,
        IEnumerable<PseudoParameter> parameters
    )
    {
        Name = name;
        Modifiers = modifiers.ToImmutableList();
        ReturnType = returnType;
        Parameters = parameters.ToImmutableList();
    }
}

public record PseudoParameter
{
    public NonEmptyString Name { get; init; }

    public PseudoType Type { get; init; }

    public PseudoParameter(NonEmptyString name, PseudoType type)
    {
        Name = name;
        Type = type;
    }
}

public record PseudoField : PseudoMember
{
    public PseudoField(
        NonEmptyString name,
        IEnumerable<PseudoModifier> modifiers,
        PseudoType type,
        string? value = null
    )
        : base(name, modifiers, type, value)
    {}
}

public record PseudoType
{
    public NonEmptyString TypeName { get; init; }

    public PseudoType(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is not null)
        {
            TypeName = $"{type.Name} | null";
        }
        else if (type.IsGenericType)
        {
            TypeName = "Any";
        }
        else
        {
            TypeName = type.Name;
        }
    }
}

public class PseudoModifier
{
    private NonEmptyString Name { get; init; }
    
    public PseudoModifier(NonEmptyString name)
    {
        Name = name;
    }
    
    public static string Combine(IEnumerable<PseudoModifier> modifiers)
    {
        var sb = new StringBuilder();
        foreach (var modifier in modifiers)
        {
            sb.Append(modifier);
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        return $"{Name} ";
    }
}

public class PropertyMethodType
{
    private NonEmptyString Name { get; init; }

    private PropertyMethodType(NonEmptyString name)
    {
        Name = name;
    }
    
    public static PropertyMethodType Get => new("get");
    public static PropertyMethodType Set => new("set");
    public static PropertyMethodType Init => new("init");

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('{');
        sb.Append(Name);
        sb.Append(';');
        sb.Append('}');
        return sb.ToString();
    }
}
