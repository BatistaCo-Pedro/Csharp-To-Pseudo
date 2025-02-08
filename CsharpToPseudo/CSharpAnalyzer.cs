using System.Collections.Immutable;
using System.Reflection;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CsharpToPseudo;

public static class CSharpAnalyzer
{
    public static async Task<ImmutableList<TypeDeclarationSyntax>> Analyze()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var projectPath =
            assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(
                    x => string.Equals(x.Key, "ProjectPath", StringComparison.OrdinalIgnoreCase)
                )
                ?.Value ?? throw new InvalidOperationException("No project path provided.");
        
        // Create a workspace
        var manager = new AnalyzerManager();

        var analyzer = manager.GetProject(projectPath);
        using var workspace = new AdhocWorkspace();
        workspace.WorkspaceFailed += (sender, args) =>
        {
            Console.WriteLine($"{args.Diagnostic.Kind}: {args.Diagnostic.Message}");
        };

        var project = analyzer.AddToWorkspace(workspace);

        // Take the first .cs file in the project
        var csDocuments = project.Documents.Where(
            d => d.Name.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)
        );

        var compilation = await project.GetCompilationAsync();
        
        if(compilation == null)
            return [];

        var allTypesToConvert = new List<TypeDeclarationSyntax>();
        foreach (var csDocument in csDocuments)
        {
            var typeRoot = await AnalyzeDocument(csDocument, compilation);
            if (typeRoot != null)
            {
                allTypesToConvert.Add(typeRoot);
            }
        }
        
        return allTypesToConvert.ToImmutableList();
    }

    private static async Task<TypeDeclarationSyntax?> AnalyzeDocument(Document csDocument, Compilation compilation)
    {
        var syntaxTree = await csDocument.GetSyntaxTreeAsync();

        if (syntaxTree == null)
            return null;

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = await syntaxTree.GetRootAsync();

        return root.DescendantNodes()
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault(td => td.InheritsInterface(semanticModel, typeof(IAnalyzable).FullName!));
    }

    private static bool InheritsInterface(
        this TypeDeclarationSyntax typeDecl,
        SemanticModel semanticModel,
        string fullyQualifiedInterfaceName
    )
    {
        // Get the symbol for the type declaration
        if (semanticModel.GetDeclaredSymbol(typeDecl) is not ITypeSymbol typeSymbol)
            return false;

        var interfaceSymbol = semanticModel.Compilation.GetTypeByMetadataName(
            fullyQualifiedInterfaceName
        );
        return interfaceSymbol != null
            &&
            // Check if the typeSymbol implements the interface
            // 'AllInterfaces' includes interfaces implemented by base classes,
            // or via other inherited interfaces, etc.
            typeSymbol.AllInterfaces.Contains(interfaceSymbol);
    }
}