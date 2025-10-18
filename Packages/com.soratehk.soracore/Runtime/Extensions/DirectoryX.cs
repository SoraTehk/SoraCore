using System.IO;

namespace SoraTehk.Extensions {
    public static partial class DirectoryX {
        public static void EnsureDirectory(string? uri) {
            string? dir = Path.HasExtension(uri)
                ? Path.GetDirectoryName(uri)
                : uri;
            if (string.IsNullOrEmpty(dir)) return;
            
            Directory.CreateDirectory(dir);
        }
    }
}