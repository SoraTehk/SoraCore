using System;
using System.Linq;

namespace SoraTehk.Extensions {
    public static partial class TypeX {
        public static string GetFriendlyTypeName(this Type? type) {
            if (type == null) return "null";
            
            if (!type.IsGenericType) return type.Name;
            
            string typeName = type.Name;
            // Remove backtick
            typeName = typeName[..typeName.IndexOf('`')];
            
            string genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
            return $"{typeName}<{genericArgs}>";
        }
    }
}