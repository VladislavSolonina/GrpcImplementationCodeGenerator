using Microsoft.CodeAnalysis;

namespace GrpcCodeGenerator.Extentions
{
    /// <summary>
    /// Класс, содержащий некоторые расширения для этого проекта
    /// </summary>
    internal static class Extentions
    {
        /// <summary>
        /// Получение имени без пространства имён
        /// </summary>
        public static string? GetShortName(this TypedConstant typedConstant)
        {
            if (typedConstant.Value == null)
                return null;

            if(typedConstant.Value is ISymbol symbol)
            {
                return symbol.Name;
            }

            return GetName(typedConstant.Value.ToString());
        }

        /// <summary>
        /// Получение имени пространства имён
        /// </summary>
        public static string? GetNamespace(this TypedConstant typedConstant)
        {
            if (typedConstant.Value == null)
                return null;

            if (typedConstant.Value is ISymbol symbol)
            {
                return symbol.ContainingNamespace.ToString();
            }

            return null;
        }

        /// <summary>
        /// Получение последней строки, если строка имеет в себе разделение знаком '.'
        /// </summary>
        public static string GetShortName(this string str)
        {
            return GetName(str);
        }

        private static string GetName(string str)
        {
            if (!str.Contains("."))
                return str;

            return str.Split('.').Last();
        }
    }
}
