using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using AgentIO.Models;
using AgentIO.Services;
using AgentIO.Tests.Services;

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

            // Register services
            services.AddSingleton<IAliasService, TestAliasService>();
            services.AddSingleton<IDirectoryService, TestDirectoryService>();
            services.AddSingleton<IFileService, TestFileService>();
            services.AddSingleton<IKernelService, KernelService>();

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