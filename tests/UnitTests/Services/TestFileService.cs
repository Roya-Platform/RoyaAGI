using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentIO.Services;

namespace AgentIO.Tests.Services
{
    /// <summary>
    /// Test implementation of the file service interface for unit testing.
    /// </summary>
    public class TestFileService : IFileService
    {
        private readonly Dictionary<string, string> _files = new();

        public Task<bool> FileExistsAsync(string path)
        {
            return Task.FromResult(_files.ContainsKey(path));
        }

        public Task<string> ReadFileAsync(string path)
        {
            if (!_files.ContainsKey(path))
            {
                throw new FileNotFoundException($"File not found: {path}");
            }

            return Task.FromResult(_files[path]);
        }

        public Task<bool> WriteFileAsync(string path, string content)
        {
            _files[path] = content;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteFileAsync(string path)
        {
            if (!_files.ContainsKey(path))
            {
                return Task.FromResult(false);
            }

            _files.Remove(path);
            return Task.FromResult(true);
        }

        public Task<long> GetFileSizeAsync(string path)
        {
            if (!_files.ContainsKey(path))
            {
                throw new FileNotFoundException($"File not found: {path}");
            }

            return Task.FromResult((long)_files[path].Length);
        }
    }
}
