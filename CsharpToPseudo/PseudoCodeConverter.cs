using CsharpToPseudo.PseudoCode;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CsharpToPseudo;

public static class PseudoCodeConverter
{
    public static string ConvertToPseudoCode(TypeDeclarationSyntax typeRoot)
    {
        return CreatePseudoClass(typeRoot).ToString();
    }

    private static PseudoTypeDeclaration CreatePseudoClass(TypeDeclarationSyntax typeRoot)
    {
        var name = typeRoot.Identifier.Text;
        var type = typeRoot.Kind().ToString();
        var modifiers = typeRoot.Modifiers.Select(m => new PseudoModifier(m.Text));
        var properties = typeRoot.Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(CreatePseudoProperty);
        var fields = typeRoot.Members.OfType<FieldDeclarationSyntax>().Select(CreatePseudoField);
        var methods = typeRoot.Members.OfType<MethodDeclarationSyntax>().Select(CreatePseudoMethod);
        return new PseudoTypeDeclaration(name, type, modifiers, properties, fields, methods);
    }

    private static PseudoProperty CreatePseudoProperty(
        PropertyDeclarationSyntax propertyDeclaration
    )
    {
        var propertyName = propertyDeclaration.Identifier.Text;
        var propertyModifiers = propertyDeclaration.Modifiers.Select(
            m => new PseudoModifier(m.Text)
        );
        var propertyType = new PseudoType(propertyDeclaration.Type.GetType());
        var getter = propertyDeclaration.AccessorList!.Accessors.First(
            a => a.Kind() == SyntaxKind.GetAccessorDeclaration
        );
        var pseudoGetter = new PseudoPropertyMethod(
            new PseudoModifier(getter.Modifiers.First().Text),
            PropertyMethodType.Get
        );

        var setter = propertyDeclaration.AccessorList?.Accessors.FirstOrDefault(
            a => a.Kind() is SyntaxKind.SetAccessorDeclaration or SyntaxKind.InitAccessorDeclaration
        );
        PseudoPropertyMethod? pseudoSetter = null;
        if (setter is not null)
        {
             pseudoSetter = new PseudoPropertyMethod(
                new PseudoModifier(getter.Modifiers.First().Text),
                setter.Kind() is SyntaxKind.SetAccessorDeclaration
                    ? PropertyMethodType.Set
                    : PropertyMethodType.Init
            );
        }
        
        var value = propertyDeclaration.Initializer?.Value.ToString();

        return new PseudoProperty(
            propertyName,
            propertyModifiers,
            propertyType,
            pseudoGetter,
            pseudoSetter,
            value
        );
    }
    
    private static PseudoField CreatePseudoField(FieldDeclarationSyntax fieldDeclaration)
    {
        var fieldName = fieldDeclaration.Declaration.Variables.First().Identifier.Text;
        var fieldModifiers = fieldDeclaration.Modifiers.Select(m => new PseudoModifier(m.Text));
        var fieldType = new PseudoType(fieldDeclaration.Declaration.Type.GetType());
        var value = fieldDeclaration.Declaration.Variables.First().Initializer?.Value.ToString();
        return new PseudoField(fieldName, fieldModifiers, fieldType, value);
    }
    
    private static PseudoMethod CreatePseudoMethod(MethodDeclarationSyntax methodDeclaration)
    {
        var methodName = methodDeclaration.Identifier.Text;
        var methodModifiers = methodDeclaration.Modifiers.Select(m => new PseudoModifier(m.Text));
        var methodReturnType = new PseudoType(methodDeclaration.ReturnType.GetType());
        var parameters = methodDeclaration.ParameterList.Parameters.Select(CreatePseudoParameters);
        return new PseudoMethod(methodName, methodModifiers, methodReturnType, parameters);
    }

    private static PseudoParameter CreatePseudoParameters(ParameterSyntax parameterDeclaration)
    {
        var parameterName = parameterDeclaration.Identifier.Text;
        var parameterType = new PseudoType(parameterDeclaration.Type!.GetType());
        return new PseudoParameter(parameterName, parameterType);
    }
}
