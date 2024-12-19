using System;
using System.IO;
using System.Threading.Tasks;

namespace AgentIO.Services
{
    /// <summary>
    /// Implementation of the file operations service.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IDirectoryService _directoryService;

        public FileService(IDirectoryService directoryService)
        {
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
        }

        public async Task<string> ReadFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return await File.ReadAllTextAsync(filePath);
        }

        public async Task<bool> UpdateFileAsync(string filePath, int lineNumber, string content)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var lines = await File.ReadAllLinesAsync(filePath);
            
            if (lineNumber < 1 || lineNumber > lines.Length + 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lineNumber), $"Line number {lineNumber} is out of range");
            }

            var newLines = new string[lines.Length + content.Split(Environment.NewLine).Length - 1];
            
            // Copy lines before the insertion point
            Array.Copy(lines, 0, newLines, 0, lineNumber - 1);
            
            // Insert the new content
            var contentLines = content.Split(Environment.NewLine);
            for (int i = 0; i < contentLines.Length; i++)
            {
                newLines[lineNumber - 1 + i] = contentLines[i];
            }
            
            // Copy the rest of the original file
            if (lineNumber <= lines.Length)
            {
                Array.Copy(lines, lineNumber - 1, newLines, lineNumber - 1 + contentLines.Length, 
                    lines.Length - (lineNumber - 1));
            }

            await File.WriteAllLinesAsync(filePath, newLines);
            return true;
        }

        public async Task<bool> CreateFileAsync(string filePath, string content, bool createDirectories)
        {
            if (createDirectories)
            {
                await _directoryService.CreateDirectoryAsync(Path.GetDirectoryName(filePath));
            }

            await File.WriteAllTextAsync(filePath, content);
            return true;
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            File.Delete(filePath);
            return true;
        }

        public async Task<bool> FileExistsAsync(string filePath)
        {
            return File.Exists(filePath);
        }
    }
} 