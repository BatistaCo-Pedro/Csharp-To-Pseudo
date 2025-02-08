using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CsharpToPseudo;

public interface IPseudoCodeConverter
{
    static abstract string ConvertToPseudoCode(TypeDeclarationSyntax typeRoot);
}