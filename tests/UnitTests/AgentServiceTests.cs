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
    /// Tests for the AgentService class which is the main interface for agents to interact with the kernel.
    /// </summary>
    public class AgentServiceTests : IDisposable
    {
        private readonly Mock<IKernelService> _kernelServiceMock;
        private readonly Dictionary<string, string> _directoryAliases;
        private readonly AgentService _agentService;

        public AgentServiceTests()
        {
            // Setup test environment
            _kernelServiceMock = new Mock<IKernelService>();
            _directoryAliases = TestHelpers.CreateStandardDirectoryAliases();
            _agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
        }

        /// <summary>
        /// Tests that directory aliases are correctly resolved when reading files.
        /// </summary>
        [Fact]
        public async Task ReadFile_WithDirectoryAlias_ResolvesAliasCorrectly()
        {
            // Arrange
            var aliasPath = "My Repo/test.txt";
            var physicalPath = "C:/Users/Jeff/repo/test.txt";
            var content = "Test content";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == aliasPath && 
                    r.RequestCategory == "IO" && 
                    r.RequestType == "Read")))
                .ReturnsAsync(new KernelResponse { Content = content, Success = true });
            
            // Act
            var result = await _agentService.ReadFileAsync(aliasPath);
            
            // Assert
            result.Should().Be(content);
            _kernelServiceMock.Verify(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                r.FilePath == aliasPath)), Times.Once);
        }

        /// <summary>
        /// Tests that the agent service correctly handles file existence checks.
        /// </summary>
        [Fact]
        public async Task FileExists_ExistingFile_ReturnsTrue()
        {
            // Arrange
            var filePath = "C:/Users/Jeff/repo/exists.txt";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == filePath && 
                    r.RequestCategory == "IO" && 
                    r.RequestType == "Exists")))
                .ReturnsAsync(new KernelResponse { Success = true, Exists = true });
            
            // Act
            var result = await _agentService.FileExistsAsync(filePath);
            
            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Tests that the agent service correctly handles directory existence checks.
        /// </summary>
        [Fact]
        public async Task DirectoryExists_ExistingDirectory_ReturnsTrue()
        {
            // Arrange
            var dirPath = "C:/Users/Jeff/repo";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.Directory == dirPath && 
                    r.RequestCategory == "IO" && 
                    r.RequestType == "DirExists")))
                .ReturnsAsync(new KernelResponse { Success = true, Exists = true });
            
            // Act
            var result = await _agentService.DirectoryExistsAsync(dirPath);
            
            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Tests that the agent service correctly handles errors from the kernel service.
        /// </summary>
        [Fact]
        public async Task KernelServiceError_PropagatesException()
        {
            // Arrange
            var filePath = "C:/invalid/path.txt";
            var errorMessage = "Access denied";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == filePath && 
                    r.RequestCategory == "IO" && 
                    r.RequestType == "Read")))
                .ReturnsAsync(new KernelResponse { IsError = true, ErrorMessage = errorMessage });
            
            // Act
            Func<Task> act = async () => await _agentService.ReadFileAsync(filePath);
            
            // Assert
            await act.Should().ThrowAsync<IOException>().WithMessage("*" + errorMessage + "*");
        }

        /// <summary>
        /// Tests that the agent service correctly handles getting all directory aliases.
        /// </summary>
        [Fact]
        public void GetDirectoryAliases_ReturnsAllAliases()
        {
            // Act
            var result = _agentService.GetDirectoryAliases();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(_directoryAliases.Count);
            result.Should().ContainKey("My Repo").WhoseValue.Should().Be("C:/Users/Jeff/repo");
        }

        public void Dispose()
        {
            // Cleanup test resources
        }
    }
}
