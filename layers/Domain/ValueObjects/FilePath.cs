using System;
using System.IO;

namespace RoyaAI.Layers.Domain.ValueObjects
{
    public class FilePath
    {
        public string Value { get; }
        
        private FilePath(string path)
        {
            Value = path;
        }
        
        public static FilePath Create(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
                
            // Normalize the path
            string normalizedPath = path.Replace('\\', Path.DirectorySeparatorChar)
                                        .Replace('/', Path.DirectorySeparatorChar);
            
            return new FilePath(normalizedPath);
        }
        
        public bool IsAbsolute => Path.IsPathRooted(Value);
        
        public string GetFileName() => Path.GetFileName(Value);
        
        public string GetExtension() => Path.GetExtension(Value);
        
        public string GetDirectory() => Path.GetDirectoryName(Value);
        
        public FilePath Combine(string relativePath)
        {
            return new FilePath(Path.Combine(Value, relativePath));
        }
        
        public override string ToString() => Value;
        
        public override bool Equals(object obj)
        {
            if (obj is FilePath other)
            {
                return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
        }
    }
} 