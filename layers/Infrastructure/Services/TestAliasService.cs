using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace AgentIO.Tests.Services
{
    /// <summary>
    /// Test implementation of the alias service.
    /// </summary>
    public class TestAliasService : IAliasService
    {
        private readonly Dictionary<string, string> _aliases;

        public TestAliasService(Dictionary<string, string> aliases)
        {
            _aliases = aliases;
        }

        public Task<bool> AliasExistsAsync(string alias)
        {
            return Task.FromResult(_aliases.ContainsKey(alias));
        }

        public Task<Dictionary<string, string>> GetAliasesAsync()
        {
            return Task.FromResult(_aliases);
        }

        public Task<string> ResolvePathAsync(string path)
        {
            foreach (var alias in _aliases.Keys)
            {
                if (path.StartsWith(_aliases[alias] + "/") || path == _aliases[alias])
                {
                    return Task.FromResult(path.Replace(_aliases[alias], alias));
                }
            }

            foreach (var alias in _aliases)
            {
                if (path.StartsWith(alias.Value + "/") || path == alias.Value)
                {
                    return Task.FromResult(path.Replace(alias.Value, alias.Key));
                }
            }

            if (path.Contains("/") && !path.Contains(":"))
            {
                string[] parts = path.Split('/', 2);
                if (_aliases.TryGetValue(parts[0], out string physicalPath))
                {
                    return Task.FromResult($"{physicalPath}/{parts[1]}");
                }

                throw new DirectoryNotFoundException($"Directory alias '{parts[0]}' not found");
            }

            return Task.FromResult(path);
        }
    }
} 