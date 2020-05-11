using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace BuilderBuilder
{
    internal class TypeBuilderWriter
    {

        private readonly StringBuilder _sb = new StringBuilder();


        public string Write(ITypeSymbol type)
        {
            try
            {
                var properties = GetProperties(type);

                _sb.AppendFormat("namespace {0} {{", type.ContainingNamespace.Name)
                   .AppendLine();

                _sb.AppendFormat("{0} class {1}Builder {{", "public", type.Name);
                _sb.AppendLine();

                AppendProperties(type, properties);
                _sb.AppendLine();
                
                AppendBuildFn(type, properties);
                _sb.AppendLine();

                _sb.AppendLine("}}");
                _sb.AppendLine();

                var result = _sb.ToString();
                return result;
            }
            finally
            {
                _sb.Clear();
            }
        }

        private IPropertySymbol[] GetProperties(ITypeSymbol type)
        {
            var result = new List<IPropertySymbol>();

            foreach (var member in type.GetMembers())
            {
                if (member is IPropertySymbol propertySymbol)
                {
                    result.Add(propertySymbol);
                }
            }

            return result.ToArray(); ;
        }

        private void AppendProperties(ITypeSymbol type, IPropertySymbol[] props)
        {
            foreach (var prop in props)
            {
                _sb.AppendFormat("public {0} {1} {{ get; set; }}", prop.Type, prop.Name)
                   .AppendLine();
            }
        }

        private void AppendBuildFn(ITypeSymbol type, IPropertySymbol[] props)
        {
            _sb.AppendFormat("public {0} Build()", type.Name)
               .AppendLine("{")
               .AppendFormat("return new {0}(", type.Name)
               .AppendLine();

            foreach (var prop in props)
            {
                _sb.AppendFormat("{0}, ", prop.Name);
            }

            if(props.Length > 0)
            {
                _sb.Length -= ", ".Length;
            }

            _sb.AppendLine(");")
               .AppendLine("}");
        }

        public void Reset() => _sb.Clear();

    }
}
