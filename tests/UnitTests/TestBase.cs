using System;
using Microsoft.Extensions.DependencyInjection;

namespace AgentIO.Tests
{
    /// <summary>
    /// Base class for all tests that provides access to the service provider
    /// and common test setup/teardown functionality.
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        protected readonly ServiceProvider ServiceProvider;
        private bool _disposed = false;

        protected TestBase()
        {
            // Create a new service provider for each test
            ServiceProvider = TestDependencySetup.CreateServiceProvider();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ServiceProvider.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
