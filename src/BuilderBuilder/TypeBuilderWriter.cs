using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace BuilderBuilder;

internal static class TypeBuilderWriter
{
    public static string Write(ITypeSymbol type)
    {
        StringBuilder sb = new();
        var properties = GetProperties(type);

        sb.AppendLine("using System;")
          .AppendLine()
          .Append("namespace ")
          .Append(type.ContainingNamespace.ToDisplayString())
          .AppendLine(";")
          .Append("public static partial class ")
          .Append(type.Name)
          .AppendLine("Builder {");

        foreach (var prop in properties)
        {
            sb.Append("public ").Append(prop.Type).Append(' ').Append(prop.Name).AppendLine(" { get; set; }");
        }
        sb.AppendLine();

        AppendBuildMethod(sb, type.Name, properties);
        sb.AppendLine()
           .AppendLine("}")
           .AppendLine();

        return sb.ToString();
    }

    private static IEnumerable<IPropertySymbol> GetProperties(ITypeSymbol type)
    {
        foreach (var member in type.GetMembers())
        {
            if (member is IPropertySymbol propertySymbol)
                yield return propertySymbol;
        }
    }

    private static void AppendBuildMethod(StringBuilder sb, string typeName, IEnumerable<IPropertySymbol> props)
    {
        const string Separator = ", ";

        sb
        .Append("public ")
        .Append(typeName)
        .AppendLine(" Build() =>")
           .Append("    new ")
           .Append(typeName)
           .Append('(');

        foreach (var prop in props)
            sb.Append(prop.Name).Append(Separator);

        if (props.Any())
            sb.Length -= Separator.Length;

        sb.AppendLine(");");
    }
}
