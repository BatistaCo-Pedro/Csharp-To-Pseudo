namespace CsharpToPseudo.PseudoCode;

public readonly record struct PseudoModifier(ReadOnlyMemory<char> Name)
{
    public override string ToString() => new(Name.Span);

    // Combine a collection of modifiers into a single string.
    public static string Combine(ReadOnlyMemory<PseudoModifier> modifiers)
    {
        int totalLength = 0;
        var modSpan = modifiers.Span;
        for (int i = 0; i < modSpan.Length; i++)
        {
            totalLength += modSpan[i].Name.Span.Length + 1;
        }

        Span<char> buffer = totalLength <= 256 
            ? stackalloc char[totalLength] 
            : new char[totalLength];

        int pos = 0;
        for (int i = 0; i < modSpan.Length; i++)
        {
            ReadOnlySpan<char> modName = modSpan[i].Name.Span;
            modName.CopyTo(buffer[pos..]);
            pos += modName.Length;
            buffer[pos++] = ' ';
        }
        return new string(buffer[..pos]);
    }
}

public readonly record struct PseudoType(ReadOnlyMemory<char> TypeName)
{
    public override string ToString() => new(TypeName.Span);
}

public readonly record struct PropertyMethodType(ReadOnlyMemory<char> Name)
{
    public override string ToString() => new(Name.Span);

    public static PropertyMethodType Get => new("get".AsMemory());
    public static PropertyMethodType Set => new("set".AsMemory());
    public static PropertyMethodType Init => new("init".AsMemory());
}

public readonly record struct PseudoPropertyMethod(PseudoModifier Modifier, PropertyMethodType MethodType)
{
    public override string ToString() =>
        string.Concat(Modifier.ToString(), MethodType.ToString(), ";");
}

public readonly record struct PseudoParameter(ReadOnlyMemory<char> Name, PseudoType Type)
{
    public override string ToString() =>
        string.Concat(Type.ToString(), " ", new string(Name.Span));
}

public readonly record struct PseudoMethod(
    ReadOnlyMemory<char> Name,
    ReadOnlyMemory<PseudoModifier> Modifiers,
    PseudoType ReturnType,
    bool HasReturnType,
    ReadOnlyMemory<PseudoParameter> Parameters)
{
    public string ToPseudoString()
    {
        const int bufferSize = 1024 * 10;
        Span<char> buffer = stackalloc char[bufferSize];
        int pos = 0;

        // Append method modifiers.
        var mods = Modifiers.Span;
        for (int i = 0; i < mods.Length; i++)
        {
            string modStr = mods[i].ToString();
            modStr.AsSpan().CopyTo(buffer[pos..]);
            pos += modStr.Length;
            buffer[pos++] = ' ';
        }

        // Append return type if present.
        if (HasReturnType)
        {
            string retStr = ReturnType.ToString();
            retStr.AsSpan().CopyTo(buffer[pos..]);
            pos += retStr.Length;
            buffer[pos++] = ' ';
        }

        // Append method name.
        string nameStr = new string(Name.Span);
        nameStr.AsSpan().CopyTo(buffer[pos..]);
        pos += nameStr.Length;
        buffer[pos++] = '(';

        // Append parameters.
        var paramsSpan = Parameters.Span;
        for (int i = 0; i < paramsSpan.Length; i++)
        {
            string paramStr = paramsSpan[i].ToString();
            paramStr.AsSpan().CopyTo(buffer[pos..]);
            pos += paramStr.Length;
            if (i < paramsSpan.Length - 1)
            {
                buffer[pos++] = ',';
                buffer[pos++] = ' ';
            }
        }
        buffer[pos++] = ')';

        // Append trailing text.
        const string trailing = " { ... }";
        trailing.AsSpan().CopyTo(buffer[pos..]);
        pos += trailing.Length;
        return new string(buffer[..pos]);
    }
    public override string ToString() => ToPseudoString();
}

public readonly record struct PseudoField(
    ReadOnlyMemory<char> Name,
    ReadOnlyMemory<PseudoModifier> Modifiers,
    PseudoType FieldType,
    ReadOnlyMemory<char> Value) // if Value is empty, no initializer.
{
    public string ToPseudoString()
    {
        const int bufferSize = 1024;
        Span<char> buffer = stackalloc char[bufferSize];
        int pos = 0;

        // Append field modifiers.
        var mods = Modifiers.Span;
        for (int i = 0; i < mods.Length; i++)
        {
            string modStr = mods[i].ToString();
            modStr.AsSpan().CopyTo(buffer[pos..]);
            pos += modStr.Length;
            buffer[pos++] = ' ';
        }
        // Append field type.
        string typeStr = FieldType.ToString();
        typeStr.AsSpan().CopyTo(buffer[pos..]);
        pos += typeStr.Length;
        buffer[pos++] = ' ';
        // Append field name.
        string nameStr = new string(Name.Span);
        nameStr.AsSpan().CopyTo(buffer[pos..]);
        pos += nameStr.Length;
        // Append initializer if any.
        if (Value.Length > 0)
        {
            buffer[pos++] = ' ';
            buffer[pos++] = '=';
            buffer[pos++] = ' ';
            string valueStr = new string(Value.Span);
            valueStr.AsSpan().CopyTo(buffer[pos..]);
            pos += valueStr.Length;
        }
        buffer[pos++] = ';';
        return new string(buffer[..pos]);
    }
    public override string ToString() => ToPseudoString();
}

public readonly record struct PseudoProperty(
    ReadOnlyMemory<char> Name,
    ReadOnlyMemory<PseudoModifier> Modifiers,
    PseudoType PropertyType,
    PseudoPropertyMethod Getter,
    bool HasSetter,
    PseudoPropertyMethod? Setter,
    ReadOnlyMemory<char> Value) // optional initializer value.
{
    public string ToPseudoString()
    {
        const int bufferSize = 1024;
        Span<char> buffer = stackalloc char[bufferSize];
        int pos = 0;

        // Append property modifiers.
        var mods = Modifiers.Span;
        for (int i = 0; i < mods.Length; i++)
        {
            string modStr = mods[i].ToString();
            modStr.AsSpan().CopyTo(buffer[pos..]);
            pos += modStr.Length;
            buffer[pos++] = ' ';
        }
        // Append property type.
        string typeStr = PropertyType.ToString();
        typeStr.AsSpan().CopyTo(buffer[pos..]);
        pos += typeStr.Length;
        buffer[pos++] = ' ';
        // Append property name.
        string nameStr = new string(Name.Span);
        nameStr.AsSpan().CopyTo(buffer[pos..]);
        pos += nameStr.Length;
        buffer[pos++] = ' ';

        // Append property accessors.
        buffer[pos++] = '{';
        buffer[pos++] = ' ';
        string getterStr = Getter.ToString();
        getterStr.AsSpan().CopyTo(buffer[pos..]);
        pos += getterStr.Length;
        if (HasSetter)
        {
            buffer[pos++] = ' ';
            string setterStr = Setter.ToString();
            setterStr.AsSpan().CopyTo(buffer[pos..]);
            pos += setterStr.Length;
        }
        buffer[pos++] = ' ';
        buffer[pos++] = '}';

        // Append initializer if any.
        if (Value.Length > 0)
        {
            buffer[pos++] = ' ';
            buffer[pos++] = '=';
            buffer[pos++] = ' ';
            string valueStr = new string(Value.Span);
            valueStr.AsSpan().CopyTo(buffer[pos..]);
            pos += valueStr.Length;
            buffer[pos++] = ';';
        }
        return new string(buffer[..pos]);
    }
    public override string ToString() => ToPseudoString();
}

public readonly record struct PseudoConstructor(
    ReadOnlyMemory<char> Name, // typically the same as the type name.
    ReadOnlyMemory<PseudoModifier> Modifiers,
    ReadOnlyMemory<PseudoParameter> Parameters)
{
    public string ToPseudoString()
    {
        const int bufferSize = 1024;
        Span<char> buffer = stackalloc char[bufferSize];
        int pos = 0;

        // Append constructor modifiers.
        var mods = Modifiers.Span;
        for (int i = 0; i < mods.Length; i++)
        {
            string modStr = mods[i].ToString();
            modStr.AsSpan().CopyTo(buffer[pos..]);
            pos += modStr.Length;
            buffer[pos++] = ' ';
        }
        // Append constructor name.
        string nameStr = new string(Name.Span);
        nameStr.AsSpan().CopyTo(buffer[pos..]);
        pos += nameStr.Length;
        buffer[pos++] = '(';
        // Append parameters.
        var paramsSpan = Parameters.Span;
        for (int i = 0; i < paramsSpan.Length; i++)
        {
            string paramStr = paramsSpan[i].ToString();
            paramStr.AsSpan().CopyTo(buffer[pos..]);
            pos += paramStr.Length;
            if (i < paramsSpan.Length - 1)
            {
                buffer[pos++] = ',';
                buffer[pos++] = ' ';
            }
        }
        buffer[pos++] = ')';
        const string trailing = " { ... }";
        trailing.AsSpan().CopyTo(buffer[pos..]);
        pos += trailing.Length;
        return new string(buffer[..pos]);
    }
    public override string ToString() => ToPseudoString();
}

public readonly record struct PseudoTypeDeclaration(
    ReadOnlyMemory<char> TypeKeyword, // e.g. "class", "struct", etc.
    ReadOnlyMemory<char> Name,
    ReadOnlyMemory<PseudoModifier> Modifiers,
    ReadOnlyMemory<PseudoField> Fields,
    ReadOnlyMemory<PseudoProperty> Properties,
    ReadOnlyMemory<PseudoConstructor> Constructors,
    ReadOnlyMemory<PseudoMethod> Methods
)
{
    public string ToPseudoString()
    {
        const int bufferSize = 1024 * 10;
        Span<char> buffer = stackalloc char[bufferSize];
        int pos = 0;

        // Append type modifiers.
        var mods = Modifiers.Span;
        for (int i = 0; i < mods.Length; i++)
        {
            string modStr = mods[i].ToString();
            modStr.AsSpan().CopyTo(buffer[pos..]);
            pos += modStr.Length;
            buffer[pos++] = ' ';
        }

        // Append type keyword and name.
        string typeStr = new string(TypeKeyword.Span);
        typeStr.AsSpan().CopyTo(buffer[pos..]);
        pos += typeStr.Length;
        buffer[pos++] = ' ';
        string nameStr = new string(Name.Span);
        nameStr.AsSpan().CopyTo(buffer[pos..]);
        pos += nameStr.Length;
        buffer[pos++] = '\n';
        buffer[pos++] = '{';
        buffer[pos++] = '\n';

        // Append fields.
        var fieldsSpan = Fields.Span;
        for (int i = 0; i < fieldsSpan.Length; i++)
        {
            buffer[pos++] = '\t';
            string fieldStr = fieldsSpan[i].ToPseudoString();
            fieldStr.AsSpan().CopyTo(buffer[pos..]);
            pos += fieldStr.Length;
            buffer[pos++] = '\n';
        }

        // Append properties.
        var propsSpan = Properties.Span;
        for (int i = 0; i < propsSpan.Length; i++)
        {
            buffer[pos++] = '\t';
            string propStr = propsSpan[i].ToPseudoString();
            propStr.AsSpan().CopyTo(buffer[pos..]);
            pos += propStr.Length;
            buffer[pos++] = '\n';
        }

        // Append constructors.
        var ctorsSpan = Constructors.Span;
        for (int i = 0; i < ctorsSpan.Length; i++)
        {
            buffer[pos++] = '\t';
            string ctorStr = ctorsSpan[i].ToPseudoString();
            ctorStr.AsSpan().CopyTo(buffer[pos..]);
            pos += ctorStr.Length;
            buffer[pos++] = '\n';
        }

        // Append methods.
        var methodsSpan = Methods.Span;
        for (int i = 0; i < methodsSpan.Length; i++)
        {
            buffer[pos++] = '\t';
            string methodStr = methodsSpan[i].ToPseudoString();
            methodStr.AsSpan().CopyTo(buffer[pos..]);
            pos += methodStr.Length;
            buffer[pos++] = '\n';
        }
        buffer[pos++] = '}';
        return new string(buffer[..pos]);
    }

    public override string ToString() => ToPseudoString();
}
