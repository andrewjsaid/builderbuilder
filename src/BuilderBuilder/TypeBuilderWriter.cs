using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace BuilderGenerator;

internal static class TypeBuilderWriter
{
    public static string Write(ITypeSymbol type)
    {
        StringBuilder sb = new();
        var properties = GetProperties(type);
        var typeName = type.Name;

        sb.AppendLine("using System;")
          .AppendLine()
          .Append("namespace ")
          .Append(type.ContainingNamespace.ToDisplayString())
          .AppendLine(";")
          .AppendLine()
          .Append("public class ")
          .Append(typeName)
          .AppendLine("Builder")
          .AppendLine("{");

        foreach (var prop in properties)
        {
            Indent(sb, 4);
            sb.Append("public ").Append(prop.Type).Append(' ').Append(prop.Name).AppendLine(" { get; set; }");
        }
        sb.AppendLine();

        AppendBuildMethod(sb, typeName, properties, 4);
        sb.AppendLine("}")
           .AppendLine();

        return sb.ToString();
    }

    private static void Indent(StringBuilder sb, uint spaces)
    {
        for (int i = 0; i < spaces; i++)
        {
            sb.Append(' ');
        }
    }

    private static IEnumerable<IPropertySymbol> GetProperties(ITypeSymbol type)
    {
        foreach (var member in type.GetMembers())
        {
            if (member is IPropertySymbol propertySymbol)
                yield return propertySymbol;
        }
    }

    private static void AppendBuildMethod(StringBuilder sb, string typeName, IEnumerable<IPropertySymbol> props, uint spaces)
    {
        const string Separator = ", ";

        Indent(sb, spaces);
        sb.Append("public ")
          .Append(typeName)
          .AppendLine(" Build() =>");
        Indent(sb, spaces * 2);
        sb.Append("new ")
        .Append(typeName)
        .Append('(');

        foreach (var prop in props)
            sb.Append(prop.Name).Append(Separator);

        if (props.Any())
            sb.Length -= Separator.Length;

        sb.AppendLine(");");
    }
}
