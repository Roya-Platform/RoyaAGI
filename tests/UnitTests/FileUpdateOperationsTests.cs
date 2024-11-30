using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentIO.Models;
using AgentIO.Services;
using Xunit;
using Moq;
using FluentAssertions;

namespace AgentIO.Tests
{
    /// <summary>
    /// Tests for file update operations at specific locations.
    /// These tests validate the kernel's ability to enable agents to modify files,
    /// which is a central part of the IO actions described in Introduction.feature:
    /// "IO actions: Like read and write of specific files".
    /// This capability allows agents to adapt configuration files, update code,
    /// and modify system settings based on client requests.
    /// </summary>
    public class FileUpdateOperationsTests : IDisposable
    {
        private readonly Mock<IKernelService> _kernelServiceMock;
        private readonly Dictionary<string, string> _directoryAliases;

        public FileUpdateOperationsTests()
        {
            // Setup test environment
            _kernelServiceMock = new Mock<IKernelService>();
            _directoryAliases = new Dictionary<string, string>
            {
                { "C:/Users/Jeff/repo", "My Repo" }
            };
        }

        /// <summary>
        /// Tests updating file content at a specific location.
        /// This validates the agent's ability to make targeted modifications to files,
        /// which is essential for implementing changes requested by the client
        /// as part of the IO actions capability specified in Introduction.feature.
        /// The precision of line-specific updates helps maintain file integrity.
        /// </summary>
        [Fact]
        public async Task UpdateFileContent_AtSpecificLocation_UpdatesSuccessfullyAsync()
        {
            // Arrange
            var content = @"// Adding a new method
public void NewFeature() {
    Console.WriteLine(""New feature added!"");
}";
            
            var request = new KernelRequest
            {
                FilePath = "C:/Users/Jeff/repo/Project1/src/main.cs",
                RequestCategory = "IO",
                RequestType = "Update",
                LineNumber = 5,
                Content = content
            };
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == request.FilePath && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType &&
                    r.LineNumber == request.LineNumber &&
                    r.Content == request.Content)))
                .ReturnsAsync(new KernelResponse { Success = true });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            var result = await agentService.UpdateFileAsync(request.FilePath, request.LineNumber, request.Content);
            
            // Assert
            result.Should().BeTrue();
            _kernelServiceMock.Verify(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                r.FilePath == request.FilePath && 
                r.LineNumber == request.LineNumber)), Times.Once);
        }

        /// <summary>
        /// Tests updating a file with complex multi-line changes
        /// </summary>
        [Fact]
        public void UpdateFileContent_WithComplexMultiLineChanges_UpdatesSuccessfully()
        {
            // Arrange
            var content = @"### PUT /api/v1/users/{id}
Updates an existing user.

### DELETE /api/v1/users/{id}
Deletes a user.";
            
            var request = new KernelRequest
            {
                FilePath = "My Repo/Project2/docs/api.md",
                RequestCategory = "IO",
                RequestType = "Update",
                LineNumber = 7,
                Content = content
            };
            
            _kernelServiceMock.Setup(x => x.SendRequest(It.Is<KernelRequest>(r => 
                r.FilePath == request.FilePath && 
                r.RequestCategory == request.RequestCategory && 
                r.RequestType == request.RequestType &&
                r.LineNumber == request.LineNumber &&
                r.Content == request.Content)))
                .Returns(new KernelResponse { Success = true });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            var result = agentService.UpdateFile(request.FilePath, request.LineNumber, request.Content);
            
            // Assert
            result.Should().BeTrue();
            _kernelServiceMock.Verify(x => x.SendRequest(It.Is<KernelRequest>(r => 
                r.FilePath == request.FilePath && 
                r.LineNumber == request.LineNumber)), Times.Once);
        }

        public void Dispose()
        {
            // Cleanup test resources
        }
    }
} 