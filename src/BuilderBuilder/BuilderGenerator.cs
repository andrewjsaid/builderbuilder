using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace BuilderBuilder
{
    [Generator]
    public class BuilderGenerator : ISourceGenerator
    {
        private const string _attributeText = @"
using System;

namespace BuilderBuilder
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class BuildableAttribute : Attribute { }
}
";

        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new BuildableReceiver());
        }

        public void Execute(SourceGeneratorContext context)
        {
            var sourceText = SourceText.From(_attributeText, Encoding.UTF8);
            context.AddSource("BuildableAttribute.cs", sourceText);

            // we're going to create a new compilation that contains the attribute.
            // TODO: we should allow source generators to provide source during initialize, so that this step isn't required.
            CSharpParseOptions options = (CSharpParseOptions)((CSharpCompilation)context.Compilation).SyntaxTrees[0].Options;
            Compilation compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(sourceText, options));

            // The above copied from https://github.com/dotnet/roslyn-sdk/blob/master/samples/CSharp/SourceGenerators/SourceGeneratorSamples/AutoNotifyGenerator.cs

            if (context.CancellationToken.IsCancellationRequested)
                return;

            var receiver = (BuildableReceiver)context.SyntaxReceiver!;
            var buildableSymbol = compilation.GetTypeByMetadataName("BuilderBuilder.BuildableAttribute");

            foreach (var @class in receiver.CandidateClasses)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    return;

                var model = compilation.GetSemanticModel(@class.SyntaxTree, true);
                var typeSymbol = model.GetDeclaredSymbol(@class);

                var isBuildable = HasAttribute(typeSymbol, buildableSymbol);
                if (isBuildable)
                {
                    Execute(context, typeSymbol);
                }
            }
        }

        private bool HasAttribute(INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeSymbol)
        {
            foreach (var attribute in typeSymbol.GetAttributes())
            {
                if (attribute.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) == true)
                {
                    return true;
                }
            }
            return false;
        }

        private void Execute(SourceGeneratorContext context, INamedTypeSymbol typeSymbol)
        {
            var writer = new TypeBuilderWriter();
            var source = writer.Write(typeSymbol);
            var sourceText = SourceText.From(source, Encoding.UTF8);
            context.AddSource($"{typeSymbol.Name}Builder.cs", sourceText);
        }
    }

}
