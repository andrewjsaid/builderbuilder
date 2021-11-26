using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace BuilderBuilder
{
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
            context.RegisterForPostInitialization(i => i.AddSource("BuildableAttribute", AttributeText));
            context.RegisterForSyntaxNotifications(() => new BuildableReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not BuildableReceiver receiver)
                return;
            Compilation compilation = context.Compilation;

            if (context.CancellationToken.IsCancellationRequested)
                return;

            var buildableSymbol = compilation.GetTypeByMetadataName("BuilderBuilder.BuildableAttribute");

            foreach (var @class in receiver.CandidateClasses)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    return;

                var model = compilation.GetSemanticModel(@class.SyntaxTree, true);
                var typeSymbol = model.GetDeclaredSymbol(@class);

                if (HasAttribute(typeSymbol, buildableSymbol))
                {
                    Execute(context, typeSymbol);
                }
            }
        }

        private static bool HasAttribute(INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeSymbol)
        {
            foreach (var attribute in typeSymbol.GetAttributes())
            {
                if (attribute.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) ?? false)
                {
                    return true;
                }
            }
            return false;
        }

        private static void Execute(GeneratorExecutionContext context, INamedTypeSymbol typeSymbol)
        {
            var source = TypeBuilderWriter.Write(typeSymbol);
            var sourceText = SourceText.From(source, Encoding.UTF8);
            context.AddSource($"{typeSymbol.Name}Builder.cs", sourceText);
        }
    }
}
