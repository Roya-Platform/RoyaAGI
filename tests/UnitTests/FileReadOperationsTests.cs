using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AgentIO.Models;
using AgentIO.Services;
using Xunit;
using Moq;
using FluentAssertions;

namespace AgentIO.Tests
{
    /// <summary>
    /// Tests for file read operations using both physical paths and aliases.
    /// These tests validate the kernel's IO capabilities for agents to read file content,
    /// which is a fundamental part of the IO actions specified in Introduction.feature:
    /// "IO actions: Like read and write of specific files, this could be applied for other IO connections on the OS".
    /// This functionality supports node health checks at startup, particularly "files health check".
    /// </summary>
    public class FileReadOperationsTests : IDisposable
    {
        private readonly Mock<IKernelService> _kernelServiceMock;
        private readonly Dictionary<string, string> _directoryAliases;

        public FileReadOperationsTests()
        {
            // Setup test environment
            _kernelServiceMock = new Mock<IKernelService>();
            _directoryAliases = new Dictionary<string, string>
            {
                { "C:/Users/Jeff/repo", "My Repo" }
            };
        }

        /// <summary>
        /// Tests reading file content using a physical path.
        /// This validates the agent's ability to access and read files through the kernel,
        /// which is part of the IO actions capability described in Introduction.feature.
        /// File access is critical for agents to understand configuration and system state.
        /// </summary>
        [Fact]
        public async Task ReadFileContent_UsingPhysicalPath_ReturnsCorrectContentAsync()
        {
            // Arrange
            var request = new KernelRequest
            {
                FilePath = "C:/Users/Jeff/repo/Project1/README.md",
                RequestCategory = "IO",
                RequestType = "Read"
            };
            
            var expectedContent = @"# Project1

This is a sample project demonstrating the agent file operations.

## Features
- Feature 1
- Feature 2

## Getting Started
See the documentation for more details.";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == request.FilePath && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType)))
                .ReturnsAsync(new KernelResponse { Content = expectedContent });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            var result = await agentService.ReadFileAsync(request.FilePath);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedContent);
        }

        /// <summary>
        /// Tests reading file content using a directory alias.
        /// Directory aliases provide simplified access to file paths, enhancing agent usability.
        /// This capability supports the "file hierarchy of the OS" understanding required at startup
        /// and the general IO actions specified in Introduction.feature.
        /// </summary>
        [Fact]
        public async Task ReadFileContent_UsingDirectoryAlias_ReturnsCorrectContentAsync()
        {
            // Arrange
            var request = new KernelRequest
            {
                FilePath = "My Repo/Project2/config.json",
                RequestCategory = "IO",
                RequestType = "Read"
            };
            
            var expectedContent = @"{
  ""name"": ""Project2"",
  ""version"": ""1.0.0"",
  ""description"": ""Sample configuration"",
  ""settings"": {
    ""timeout"": 30,
    ""maxRetries"": 3,
    ""debug"": false
  }
}";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == request.FilePath && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType)))
                .ReturnsAsync(new KernelResponse { Content = expectedContent });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            var result = await agentService.ReadFileAsync(request.FilePath);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedContent);
        }

        /// <summary>
        /// Tests handling error when file does not exist.
        /// This validates the error handling mechanism for the IO operations,
        /// which is critical for the reliability of the kernel's IO capabilities and 
        /// the "node health check" requirement from startup.feature.
        /// </summary>
        [Fact]
        public async Task ReadFileContent_NonExistentFile_ThrowsFileNotFoundExceptionAsync()
        {
            // Arrange
            var request = new KernelRequest
            {
                FilePath = "C:/Users/Jeff/repo/nonexistent.txt",
                RequestCategory = "IO",
                RequestType = "Read"
            };
            
            var errorMessage = "File not found: C:/Users/Jeff/repo/nonexistent.txt";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == request.FilePath && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType)))
                .ReturnsAsync(new KernelResponse { IsError = true, ErrorMessage = errorMessage });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            Func<Task> act = async () => await agentService.ReadFileAsync(request.FilePath);
            
            // Assert
            await act.Should().ThrowAsync<FileNotFoundException>().WithMessage(errorMessage);
        }

        /// <summary>
        /// Tests handling error when directory alias is invalid.
        /// This validates the alias resolution mechanism and appropriate error handling,
        /// ensuring the kernel provides clear feedback when directory aliases are misconfigured.
        /// This relates to the platform configuration check at startup.
        /// </summary>
        [Fact]
        public async Task ReadFileContent_InvalidDirectoryAlias_ThrowsDirectoryNotFoundExceptionAsync()
        {
            // Arrange
            var request = new KernelRequest
            {
                FilePath = "Invalid Alias/config.json",
                RequestCategory = "IO",
                RequestType = "Read"
            };
            
            var errorMessage = "Directory alias 'Invalid Alias' not found";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == request.FilePath && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType)))
                .ReturnsAsync(new KernelResponse { IsError = true, ErrorMessage = errorMessage });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            Func<Task> act = async () => await agentService.ReadFileAsync(request.FilePath);
            
            // Assert
            await act.Should().ThrowAsync<DirectoryNotFoundException>().WithMessage(errorMessage);
        }

        public void Dispose()
        {
            // Cleanup test resources
        }
    }
} 