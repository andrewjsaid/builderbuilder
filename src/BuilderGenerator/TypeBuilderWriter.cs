using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using static BuilderGenerator.BuilderGeneratorHelper;

namespace BuilderGenerator;

internal static class TypeBuilderWriter
{
    private static readonly string s_toolName = typeof(BuilderGenerator).Assembly.GetName().Name;

    public static string Write(INamedTypeSymbol type)
    {
        StringBuilder sb = new();
        IEnumerable<IPropertySymbol> properties = GetProperties(type);

        const string version = "v1.0.0.0";

        _ = sb.AppendLine(Header)
            .AppendLine("using System;")
            .AppendLine()
            .Append("namespace ")
            .Append(type.ContainingNamespace.ToDisplayString())
            .AppendLine(";")
            .AppendLine()
          // See https://github.com/dotnet/runtime/issues/64541.
            .Append("[System.CodeDom.Compiler.GeneratedCode(\"")
            .Append(s_toolName)
            .Append("\", \"")
            .Append(version)
            .AppendLine("\")]")
            .Append("public class ")
            .Append(GetTypeName(type, true))
            .AppendLine("{");

        foreach (var prop in properties)
        {
            Indent(sb, 4);
            sb.Append("public ").Append(prop.Type).Append(' ').Append(prop.Name).AppendLine(" { get; set; }");
        }
        sb.AppendLine();

        AppendBuildMethod(sb, GetTypeName(type, false), properties, 4);
        sb.AppendLine("}")
            .AppendLine();

        return sb.ToString();
    }

    private static string GetTypeName(INamedTypeSymbol type, bool isBuilder)
    {
        var typeName = type.Name;

        if (type.IsGenericType && !type.IsUnboundGenericType)
        {
            var parts = type.ToDisplayParts();
            var length = parts.Length;
            if (length > 0)
            {
                var vals = new List<string>(length);
                bool capture = false;
                for (var i = 0; i < length; i++)
                {
                    var val = parts[i].ToString();
                    if (!capture && val == typeName && i + 2 < length && parts[i + 1].ToString() == "<")
                    {
                        capture = true;
                        if (isBuilder)
                            val += "Builder";
                    }
                    if (capture)
                        vals.Add(val);
                }
                return string.Concat(vals);
            }
        }

        return isBuilder ? typeName + "Builder" : typeName;
    }

    private static void Indent(StringBuilder sb, uint spaces)
    {
        for (int i = 0; i < spaces; i++)
        {
            sb.Append(' ');
        }
    }

    private static IEnumerable<IPropertySymbol> GetProperties(INamedTypeSymbol type)
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
