using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

        if (context.CancellationToken.IsCancellationRequested)
            return;

        foreach (var @class in receiver.CandidateClasses)
        {
            if (context.CancellationToken.IsCancellationRequested)
                return;

            var model = compilation.GetSemanticModel(@class.SyntaxTree, true);
            var typeSymbol = model.GetDeclaredSymbol(@class);

            Execute(context, typeSymbol);
        }
    }

    private static void Execute(GeneratorExecutionContext context, INamedTypeSymbol typeSymbol)
    {
        var source = TypeBuilderWriter.Write(typeSymbol);
        context.AddSource($"{typeSymbol.Name}Builder.cs", source);
    }
}
