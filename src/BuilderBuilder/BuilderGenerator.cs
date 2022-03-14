using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace BuilderBuilder;

[Generator]
public class BuilderGenerator : ISourceGenerator
{
    private const string AttributeText = @"
using System;

namespace BuilderBuilder
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class BuildableAttribute : Attribute { }
}
";

    private static readonly DiagnosticDescriptor ErrorGeneratingBuilderSource = new
    (
        id: "BB001",
        title: "An error has occured while generating source for builder",
        messageFormat: "An error has occured while generating source for builder with name `{0}`: {1}",
        category: "Compilation",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor SuccessfullyGeneratedBuilderSource = new
    (
        id: "BB002",
        title: "Successfully generated source for builder",
        messageFormat: "Successfully generated source for builder with name `{0}`",
        category: "Compilation",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new BuildableReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("BuildableAttribute.cs", AttributeText);
        CSharpParseOptions options = (CSharpParseOptions)((CSharpCompilation)context.Compilation).SyntaxTrees[0].Options;
        Compilation compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(AttributeText, options));

        if (context.SyntaxContextReceiver is not BuildableReceiver receiver)
            return;

        var buildableSymbol = compilation.GetTypeByMetadataName("BuilderBuilder.BuildableAttribute");

        foreach (var @class in receiver.CandidateClasses)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return;

            var model = compilation.GetSemanticModel(@class.SyntaxTree, true);
            var typeSymbol = model.GetDeclaredSymbol(@class);

            if (HasAttribute(typeSymbol, buildableSymbol))
                Execute(context, typeSymbol);
        }
    }

    private bool HasAttribute(INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeSymbol)
    {
        foreach (var attribute in typeSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) == true)
                return true;
        }
        return false;
    }

    private static void Execute(GeneratorExecutionContext context, INamedTypeSymbol typeSymbol)
    {
        try
        {
            var source = TypeBuilderWriter.Write(typeSymbol);
            var sourceText = SourceText.From(source, Encoding.UTF8);
            context.ReportDiagnostic(Diagnostic.Create(SuccessfullyGeneratedBuilderSource, Location.None, typeSymbol.Name));
            context.AddSource($"{typeSymbol.Name}Builder.cs", sourceText);
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(ErrorGeneratingBuilderSource, Location.None, typeSymbol.Name, ex.Message));
        }
    }
}
