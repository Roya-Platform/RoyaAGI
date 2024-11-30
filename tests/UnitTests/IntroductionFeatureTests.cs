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
    /// Tests that validate the core capabilities described in the Introduction feature.
    /// These tests ensure that the system meets the requirements specified in Introduction.feature,
    /// particularly around IO actions, system information access, and shell command execution.
    /// </summary>
    public class IntroductionFeatureTests : IDisposable
    {
        private readonly Mock<IKernelService> _kernelServiceMock;
        private readonly Mock<IPlatformHost> _platformHostMock;
        private readonly Dictionary<string, string> _directoryAliases;
        private readonly AgentService _agentService;

        public IntroductionFeatureTests()
        {
            // Setup test environment
            _kernelServiceMock = new Mock<IKernelService>();
            _platformHostMock = new Mock<IPlatformHost>();
            _directoryAliases = TestHelpers.CreateStandardDirectoryAliases();
            
            // Configure platform host with system information
            _platformHostMock.Setup(x => x.Configuration).Returns(new Dictionary<string, string>
            {
                { "IP", "10.1.10.101" },
                { "OS", "windows" },
                { "OS Build Version", "10.0.19045.3693" },
                { "Machine Name", "TEST-MACHINE" },
                { "User Name", "TestUser" },
                { "Current Directory", "C:\\Users\\TestUser\\Projects" }
            });
            
            _agentService = new AgentService(_kernelServiceMock.Object, _directoryAliases);
        }

        /// <summary>
        /// Tests the agent's ability to retrieve system information at startup.
        /// This validates the requirement from Introduction.feature:
        /// "At startup, the agent should request from the kernel to get brief information
        /// of the node hardware and software information including OS build information."
        /// </summary>
        [Fact]
        public async Task AgentStartup_RequestsSystemInformation_ReturnsOSBuildInfo()
        {
            // Arrange
            var request = new KernelRequest
            {
                RequestCategory = "System",
                RequestType = "Info"
            };
            
            var expectedInfo = new Dictionary<string, string>
            {
                { "OS", "windows" },
                { "OS Build Version", "10.0.19045.3693" },
                { "Machine Name", "TEST-MACHINE" },
                { "CPU", "Intel Core i7-10700K" },
                { "Memory", "32GB" },
                { "Disk Space", "512GB SSD" }
            };
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType)))
                .ReturnsAsync(new KernelResponse 
                { 
                    Success = true,
                    SystemInfo = expectedInfo
                });
            
            // Act
            var result = await _agentService.GetSystemInformationAsync();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("OS").WhoseValue.Should().Be("windows");
            result.Should().ContainKey("OS Build Version");
            result.Should().ContainKey("Machine Name");
        }

        /// <summary>
        /// Tests the agent's ability to execute shell commands.
        /// This validates the requirement from Introduction.feature:
        /// "Shell: The kernel should provide proper library for the agent in order to connect
        /// and send requests based on the client request to apply a shell command on the node."
        /// </summary>
        [Fact]
        public async Task AgentShellExecution_RunsCommand_ReturnsOutput()
        {
            // Arrange
            var command = "dir C:\\Users";
            var request = new KernelRequest
            {
                RequestCategory = "Shell",
                RequestType = "Execute",
                Command = command
            };
            
            var expectedOutput = "Directory of C:\\Users\n\n" +
                                "05/15/2023  10:23 AM    <DIR>          TestUser\n" +
                                "05/15/2023  10:23 AM    <DIR>          Public\n";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType &&
                    r.Command == command)))
                .ReturnsAsync(new KernelResponse 
                { 
                    Success = true,
                    Content = expectedOutput,
                    ExitCode = 0
                });
            
            // Act
            var result = await _agentService.ExecuteShellCommandAsync(command);
            
            // Assert
            result.Output.Should().NotBeNull();
            result.Output.Should().Contain("Directory of C:\\Users");
            result.ExitCode.Should().Be(0);
        }

        /// <summary>
        /// Tests the agent's ability to retrieve the directory tree of the OS.
        /// This validates the requirement from Introduction.feature:
        /// "At startup, the agent should request from the kernel to get brief information
        /// of the node hardware and software information including directory tree of the OS."
        /// </summary>
        [Fact]
        public async Task AgentStartup_RequestsDirectoryTree_ReturnsFileHierarchy()
        {
            // Arrange
            var rootDirectory = "C:\\";
            var request = new KernelRequest
            {
                Directory = rootDirectory,
                RequestCategory = "IO",
                RequestType = "Tree",
                Depth = 2
            };
            
            var expectedTree = "C:\\\n" +
                              "├── Program Files\\\n" +
                              "│   ├── Common Files\\\n" +
                              "│   └── Internet Explorer\\\n" +
                              "├── Program Files (x86)\\\n" +
                              "├── Users\\\n" +
                              "│   ├── Public\\\n" +
                              "│   └── TestUser\\\n" +
                              "└── Windows\\\n" +
                              "    ├── System32\\\n" +
                              "    └── SysWOW64\\\n";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.Directory == request.Directory && 
                    r.RequestCategory == request.RequestCategory && 
                    r.RequestType == request.RequestType &&
                    r.Depth == request.Depth)))
                .ReturnsAsync(new KernelResponse { Content = expectedTree, Success = true });
            
            // Act
            var result = await _agentService.GetDirectoryTreeAsync(rootDirectory, request.Depth);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().Contain("Program Files");
            result.Should().Contain("Users");
            result.Should().Contain("Windows");
        }

        /// <summary>
        /// Tests the agent's ability to access and modify files through the kernel.
        /// This validates the requirement from Introduction.feature:
        /// "IO actions: Like read and write of specific files, this could be applied for other IO connections on the OS."
        /// </summary>
        [Fact]
        public async Task AgentIOActions_ReadAndWriteFiles_SuccessfullyModifiesContent()
        {
            // Arrange - Read file
            var filePath = "C:/Users/TestUser/config.json";
            var originalContent = "{ \"setting\": \"original value\" }";
            var newContent = "{ \"setting\": \"updated value\" }";
            
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == filePath && 
                    r.RequestCategory == "IO" && 
                    r.RequestType == "Read")))
                .ReturnsAsync(new KernelResponse { Content = originalContent, Success = true });
            
            // Arrange - Write file
            _kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.FilePath == filePath && 
                    r.RequestCategory == "IO" && 
                    r.RequestType == "Update" &&
                    r.Content == newContent)))
                .ReturnsAsync(new KernelResponse { Success = true });
            
            // Act - Read file
            var readResult = await _agentService.ReadFileAsync(filePath);
            
            // Assert - Read file
            readResult.Should().NotBeNull();
            readResult.Should().Be(originalContent);
            
            // Act - Update file
            var updateResult = await _agentService.UpdateFileAsync(filePath, 1, newContent);
            
            // Assert - Update file
            updateResult.Should().BeTrue();
            
            // Verify both operations were called
            _kernelServiceMock.Verify(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                r.FilePath == filePath && r.RequestType == "Read")), Times.Once);
            
            _kernelServiceMock.Verify(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                r.FilePath == filePath && r.RequestType == "Update")), Times.Once);
        }

        public void Dispose()
        {
            // Cleanup test resources
        }
    }
}
