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
    /// Tests for directory tree operations and navigation functionality.
    /// These tests validate the kernel's ability to provide directory hierarchies to agents,
    /// which is critical for agents to understand the node environment at startup
    /// and navigate through the file system during operation.
    /// </summary>
    public class DirectoryOperationsTests : IDisposable
    {
        private readonly Mock<IKernelService> _kernelServiceMock;
        private readonly Mock<IPlatformHost> _platformHostMock;
        private readonly Dictionary<string, string> _directoryAliases;

        public DirectoryOperationsTests()
        {
            // Setup test environment
            _kernelServiceMock = new Mock<IKernelService>();
            _platformHostMock = new Mock<IPlatformHost>();
            _directoryAliases = new Dictionary<string, string>
            {
                { "C:/Users/Jeff/repo", "My Repo" }
            };
            
            // Configure platform host
            _platformHostMock.Setup(x => x.Configuration).Returns(new Dictionary<string, string>
            {
                { "IP", "10.1.10.101" },
                { "OS", "windows" },
                { "OS Build Version", "<VERSION>" }
            });
        }

        /// <summary>
        /// Tests retrieving a directory hierarchy tree with the physical path.
        /// This is critical for agent initialization as specified in the Introduction.feature,
        /// where "At startup, the agent should request from the kernel to get brief information
        /// of the node hardware and software information including directory tree of the OS".
        /// </summary>
        [Fact]
        public async Task RetrieveDirectoryHierarchyTree_WithPhysicalPath_ReturnsCorrectStructureAsync()
        {
            // Arrange
            var request = new KernelRequest
            {
                Directory = "C:/Users/Jeff/repo",
                RequestCategory = "IO",
                RequestType = "Tree"
            };
            
            var expectedStructure = @"C:/Users/Jeff/repo/
├── Project1/
│   ├── src/
│   │   ├── main.cs
│   │   └── helper.cs
│   ├── tests/
│   │   └── test.cs
│   └── README.md
├── Project2/
│   ├── docs/
│   │   └── api.md
│   └── config.json
└── notes.txt";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.Directory == request.Directory && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType)))
                .ReturnsAsync(new KernelResponse { Content = expectedStructure });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            var result = await agentService.GetDirectoryTreeAsync(request.Directory);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedStructure);
        }

        /// <summary>
        /// Tests navigating through deep directory structure with depth parameter.
        /// This capability is essential for the agent to understand the node environment
        /// and conduct file hierarchy checks as part of the node health check specified
        /// in startup.feature.
        /// </summary>
        [Fact]
        public async Task NavigateDeepDirectoryStructure_WithDepthParam_ReturnsLimitedStructureAsync()
        {
            // Arrange
            var request = new KernelRequest
            {
                Directory = "C:/Users/Jeff/repo/Project1",
                RequestCategory = "IO",
                RequestType = "Tree",
                Depth = 2
            };
            
            var expectedStructure = @"C:/Users/Jeff/repo/Project1/
├── src/
│   ├── main.cs
│   └── helper.cs
├── tests/
│   └── test.cs
├── docs/
│   └── implementation.md
└── README.md";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.Directory == request.Directory && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType &&
                    r.Depth == request.Depth)))
                .ReturnsAsync(new KernelResponse { Content = expectedStructure });
            
            // Act
            var agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
            var result = await agentService.GetDirectoryTreeAsync(request.Directory, request.Depth);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedStructure);
        }

        public void Dispose()
        {
            // Cleanup test resources
        }
    }
} 