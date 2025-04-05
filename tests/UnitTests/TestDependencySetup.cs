using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using AgentIO.Models;
using AgentIO.Services;
using AgentIO.Tests.Services;
using Moq;

namespace AgentIO.Tests
{
    /// <summary>
    /// Sets up the dependency injection container for tests.
    /// </summary>
    public static class TestDependencySetup
    {
        public static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            // Register in-memory test implementations
            services.AddSingleton<Dictionary<string, string>>(provider => 
                new Dictionary<string, string>
                {
                    { "C:/Users/Jeff/repo", "My Repo" }
                });

            // Register mocks for easier verification in tests
            services.AddSingleton(new Mock<IFileService>());
            services.AddSingleton(new Mock<IDirectoryService>());
            services.AddSingleton(new Mock<IKernelService>());
            
            // Register services with their real implementations
            services.AddSingleton<IAliasService, TestAliasService>();
            
            // Register service implementations from mocks
            services.AddSingleton<IFileService>(provider => 
                provider.GetRequiredService<Mock<IFileService>>().Object);
            services.AddSingleton<IDirectoryService>(provider => 
                provider.GetRequiredService<Mock<IDirectoryService>>().Object);
            services.AddSingleton<IKernelService>(provider => 
                provider.GetRequiredService<Mock<IKernelService>>().Object);

            // Register other test services
            services.AddSingleton<IPlatformHost, TestPlatformHost>();
            services.AddSingleton<IAgentService, AgentService>();

            return services.BuildServiceProvider();
        }
    }

    /// <summary>
    /// Test implementation of platform host interface.
    /// </summary>
    public class TestPlatformHost : IPlatformHost
    {
        public Dictionary<string, string> Configuration => new Dictionary<string, string>
        {
            { "IP", "10.1.10.101" },
            { "OS", "windows" },
            { "OS Build Version", "<VERSION>" }
        };
    }
} 
