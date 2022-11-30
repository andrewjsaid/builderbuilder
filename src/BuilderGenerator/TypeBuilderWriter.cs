using System.Collections.Generic;
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

        const string Version = "v1.0.0.0";

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
            .Append(Version)
            .AppendLine("\")]")
            .Append("public class ")
            .Append(GetTypeName(type, true))
            .AppendLine("{");

        foreach (IPropertySymbol prop in properties)
        {
            _ = sb
                .Indent(4)
                .Append("public ")
                .Append(prop.Type)
                .Append(' ')
                .Append(prop.Name)
                .AppendLine(" { get; set; }");
        }
        _ = sb.AppendLine();

        AppendBuildMethod(sb, GetTypeName(type, false), properties, 4);
        _ = sb.AppendLine("}")
            .AppendLine();

        return sb.ToString();
    }

    private static string GetTypeName(INamedTypeSymbol type, bool isBuilder)
    {
        var typeName = type.Name;

        if (type.IsGenericType && !type.IsUnboundGenericType)
        {
            System.Collections.Immutable.ImmutableArray<SymbolDisplayPart> parts = type.ToDisplayParts();
            var length = parts.Length;
            if (length > 0)
            {
                var vals = new List<string>(length);
                var capture = false;
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

    private static StringBuilder Indent(this StringBuilder sb, int spaces) => sb.Append(' ', spaces);

    private static IEnumerable<IPropertySymbol> GetProperties(INamedTypeSymbol type)
    {
        foreach (ISymbol member in type.GetMembers())
        {
            if (member is IPropertySymbol propertySymbol)
                yield return propertySymbol;
        }
    }

    private static void AppendBuildMethod(StringBuilder sb, string typeName, IEnumerable<IPropertySymbol> props, int spaces)
    {
        const string Separator = ", ";

        sb = sb
            .Indent(spaces)
            .Append("public ")
            .Append(typeName)
            .AppendLine(" Build() =>")
            .Indent(spaces * 2)
            .Append("new ")
            .Append(typeName)
            .Append('(');

        var hasProp = false;
        foreach (IPropertySymbol prop in props)
        {
            hasProp = true;
            _ = sb.Append(prop.Name).Append(Separator);
        }
        if (hasProp)
            sb.Length -= Separator.Length;

        _ = sb.AppendLine(");");
    }
}
