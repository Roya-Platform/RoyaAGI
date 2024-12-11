using Microsoft.Extensions.DependencyInjection;
using RoyaAi.Layers.Presentation.Services;
using RoyaAi.Layers.Presentation.Services.Interfaces;
using System;

namespace RoyaAi.Layers.Presentation.Extensions
{
    /// <summary>
    /// Extensions for registering services in the DI container
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all presentation layer services to the DI container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection with added services</returns>
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            // Register presentation layer services
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IStepService, StepService>();
            services.AddScoped<IArtifactService, ArtifactService>();

            return services;
        }

        /// <summary>
        /// Adds all kernel layer services to the DI container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection with added kernel services</returns>
        public static IServiceCollection AddKernelServices(this IServiceCollection services)
        {
            // Register Kernel layer services
            // Example: services.AddScoped<IKernelService, KernelService>();
            
            // Configure Semantic Kernel
            services.AddSemanticKernelServices();

            return services;
        }

        /// <summary>
        /// Adds all infrastructure layer services to the DI container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection with added infrastructure services</returns>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register Infrastructure layer services
            // Example: services.AddScoped<IStorageService, StorageService>();
            
            // Configure any data access or external service integrations
            services.AddDataServices();

            return services;
        }

        /// <summary>
        /// Adds Semantic Kernel services to the DI container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection with added Semantic Kernel services</returns>
        private static IServiceCollection AddSemanticKernelServices(this IServiceCollection services)
        {
            // Configure and register Semantic Kernel
            // This is a placeholder for actual Semantic Kernel configuration
            // Example: services.AddSingleton(kernel => CreateSemanticKernel());

            return services;
        }

        /// <summary>
        /// Adds data access services to the DI container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection with added data services</returns>
        private static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            // Register data access services
            // Example: services.AddDbContext<AppDbContext>();

            return services;
        }
    }
}

