using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentIO.Services;
using Xunit;
using Moq;
using FluentAssertions;

namespace AgentIO.Tests
{
    /// <summary>
    /// Tests for the platform host functionality which provides system information to agents.
    /// </summary>
    public class PlatformHostTests
    {
        /// <summary>
        /// Tests that the platform host correctly provides system configuration information.
        /// </summary>
        [Fact]
        public void PlatformHost_ProvidesSystemConfiguration()
        {
            // Arrange
            var platformHost = new TestPlatformHost();
            
            // Act
            var config = platformHost.Configuration;
            
            // Assert
            config.Should().NotBeNull();
            config.Should().ContainKey("OS");
            config.Should().ContainKey("IP");
            config.Should().ContainKey("OS Build Version");
        }

        /// <summary>
        /// Tests that the agent can access platform host information through the kernel.
        /// </summary>
        [Fact]
        public async Task Agent_CanAccessPlatformHostInfo_ThroughKernel()
        {
            // Arrange
            var kernelServiceMock = new Mock<IKernelService>();
            var expectedInfo = new Dictionary<string, string>
            {
                { "OS", "windows" },
                { "IP", "10.1.10.101" },
                { "OS Build Version", "10.0.19045.3693" }
            };
            
            kernelServiceMock
                .Setup(x => x.SendRequestAsync(It.Is<KernelRequest>(r => 
                    r.RequestCategory == "System" && 
                    r.RequestType == "Info")))
                .ReturnsAsync(new KernelResponse 
                { 
                    Success = true,
                    SystemInfo = expectedInfo
                });
            
            var agentService = new AgentService(kernelServiceMock.Object, new Dictionary<string, string>());
            
            // Act
            var result = await agentService.GetSystemInformationAsync();
            
            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedInfo);
        }

        /// <summary>
        /// Tests that the platform host configuration is immutable to agents.
        /// </summary>
        [Fact]
        public void PlatformHostConfiguration_IsImmutable()
        {
            // Arrange
            var platformHost = new TestPlatformHost();
            var config = platformHost.Configuration;
            
            // Act & Assert
            Action act = () => config["OS"] = "linux";
            
            // The test will pass if this throws an exception (Dictionary is read-only)
            // or if the original value is unchanged
            try
            {
                act();
                // If we get here, check that the original value is unchanged
                platformHost.Configuration["OS"].Should().Be("windows");
            }
            catch (Exception ex) when (ex is NotSupportedException || ex is InvalidOperationException)
            {
                // Expected exception for read-only dictionary
                true.Should().BeTrue(); // Just to have an assertion
            }
        }
    }
}
