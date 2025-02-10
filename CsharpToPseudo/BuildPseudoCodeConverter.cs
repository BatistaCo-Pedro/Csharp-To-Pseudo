using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CsharpToPseudo.PseudoCode;

namespace CsharpToPseudo;

public abstract class BuildPseudoCodeConverter : IPseudoCodeConverter
{
    public static string ConvertToPseudoCode(TypeDeclarationSyntax typeRoot)
    {
        // Convert a type declaration (e.g. class or struct) into pseudo code.
        return CreatePseudoTypeDeclaration(typeRoot).ToString();
    }

    private static PseudoTypeDeclaration CreatePseudoTypeDeclaration(TypeDeclarationSyntax typeRoot)
    {
        // Note: In our pseudo model the first parameter is the type keyword (e.g. "class")
        // and the second is the type name.
        var typeKeyword = typeRoot.Keyword.Text;
        var name = typeRoot.Identifier.Text;
        var modifiers = typeRoot.Modifiers.Select(m => new PseudoModifier(m.Text.AsMemory()));

        // Process each kind of member.
        var properties = typeRoot.Members
            .OfType<PropertyDeclarationSyntax>()
            .Select(CreatePseudoProperty);
        var fields = typeRoot.Members
            .OfType<FieldDeclarationSyntax>()
            .SelectMany(CreatePseudoFields);
        var constructors = typeRoot.Members
            .OfType<ConstructorDeclarationSyntax>()
            .Select(CreatePseudoConstructor);
        var methods = typeRoot.Members
            .OfType<MethodDeclarationSyntax>()
            .Select(CreatePseudoMethod);

        // Note the order: first parameter is the type keyword, second is the type name.
        return new PseudoTypeDeclaration(
            typeKeyword.AsMemory(),
            name.AsMemory(),
            modifiers.ToArray(),
            fields.ToArray(),
            properties.ToArray(),
            constructors.ToArray(),
            methods.ToArray());
    }

    private static PseudoProperty CreatePseudoProperty(PropertyDeclarationSyntax propertyDeclaration)
    {
        var propertyName = propertyDeclaration.Identifier.Text;
        var propertyModifiers = propertyDeclaration.Modifiers.Select(m => new PseudoModifier(m.Text.AsMemory()));
        // Use the type’s text so that "int" or "IEnumerable<T>" appears correctly.
        var propertyType = new PseudoType(propertyDeclaration.Type.ToString().AsMemory());

        // Get the getter accessor (throwing if missing)
        var getterSyntax = propertyDeclaration.AccessorList?.Accessors
            .FirstOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
        if (getterSyntax == null)
        {
            throw new Exception($"Property {propertyName} has no getter.");
        }
        var getterModifier = getterSyntax.Modifiers.Any() ? getterSyntax.Modifiers.First().Text : "";
        var pseudoGetter = new PseudoPropertyMethod(
            new PseudoModifier(getterModifier.AsMemory()), 
            PropertyMethodType.Get);

        // Get the setter (or init) if one exists.
        var setterSyntax = propertyDeclaration.AccessorList?.Accessors
            .FirstOrDefault(a => a.Kind() == SyntaxKind.SetAccessorDeclaration ||
                                 a.Kind() == SyntaxKind.InitAccessorDeclaration);
        PseudoPropertyMethod? pseudoSetter = null;
        if (setterSyntax != null)
        {
            var setterModifier = setterSyntax.Modifiers.Any() ? setterSyntax.Modifiers.First().Text : "";
            var methodType = setterSyntax.Kind() == SyntaxKind.SetAccessorDeclaration
                ? PropertyMethodType.Set
                : PropertyMethodType.Init;
            pseudoSetter = new PseudoPropertyMethod(new PseudoModifier(setterModifier.AsMemory()), methodType);
        }
            
        // Capture initializer value if one exists.
        var value = propertyDeclaration.Initializer?.Value.ToString() ?? "";
        return new PseudoProperty(
            propertyName.AsMemory(),
            propertyModifiers.ToArray(),
            propertyType,
            pseudoGetter,
            pseudoSetter != null,
            pseudoSetter ?? default,
            value.AsMemory());
    }

    private static IEnumerable<PseudoField> CreatePseudoFields(FieldDeclarationSyntax fieldDeclaration)
    {
        var fieldModifiers = fieldDeclaration.Modifiers.Select(m => new PseudoModifier(m.Text.AsMemory()));
        var fieldType = new PseudoType(fieldDeclaration.Declaration.Type.ToString().AsMemory());
        // FieldDeclaration may declare multiple variables in one line.
        foreach (var variable in fieldDeclaration.Declaration.Variables)
        {
            var fieldName = variable.Identifier.Text;
            var value = variable.Initializer?.Value.ToString() ?? "";
            yield return new PseudoField(
                fieldName.AsMemory(),
                fieldModifiers.ToArray(),
                fieldType,
                value.AsMemory());
        }
    }

    private static PseudoMethod CreatePseudoMethod(MethodDeclarationSyntax methodDeclaration)
    {
        var methodName = methodDeclaration.Identifier.Text.AsMemory();
        var methodModifiers = methodDeclaration.Modifiers.Select(m => new PseudoModifier(m.Text.AsMemory()));
        var methodReturnType = new PseudoType(methodDeclaration.ReturnType.ToString().AsMemory());
        var parameters = methodDeclaration.ParameterList.Parameters.Select(CreatePseudoParameter);
        // If the return type is "void", set HasReturnType to false.
        bool hasReturnType = methodDeclaration.ReturnType.ToString() != "void";
        return new PseudoMethod(
            methodName,
            methodModifiers.ToArray(),
            methodReturnType,
            hasReturnType,
            parameters.ToArray());
    }

    private static PseudoConstructor CreatePseudoConstructor(ConstructorDeclarationSyntax constructorDeclaration)
    {
        var constructorName = constructorDeclaration.Identifier.Text.AsMemory();
        var modifiers = constructorDeclaration.Modifiers.Select(m => new PseudoModifier(m.Text.AsMemory()));
        var parameters = constructorDeclaration.ParameterList.Parameters.Select(CreatePseudoParameter);
        return new PseudoConstructor(
            constructorName,
            modifiers.ToArray(),
            parameters.ToArray());
    }

    private static PseudoParameter CreatePseudoParameter(ParameterSyntax parameterDeclaration)
    {
        var parameterName = parameterDeclaration.Identifier.Text.AsMemory();
        var parameterType = new PseudoType(parameterDeclaration.Type!.ToString().AsMemory());
        return new PseudoParameter(parameterName, parameterType);
    }
}