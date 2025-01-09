using System;
using System.Collections.Generic;
using AgentIO.Services;
using Xunit;
using Moq;
using FluentAssertions;

namespace AgentIO.Tests
{
    /// <summary>
    /// Tests for file creation and deletion operations
    /// </summary>
    public class FileCreateDeleteOperationsTests : IDisposable
    {
        private readonly Mock<IKernelService> _kernelServiceMock;
        private readonly Dictionary<string, string> _directoryAliases;

        public FileCreateDeleteOperationsTests()
        {
            // Setup test environment
            _kernelServiceMock = new Mock<IKernelService>();
            _directoryAliases = new Dictionary<string, string>
            {
                { "C:/Users/Jeff/repo", "My Repo" }
            };
        }

        /// <summary>
        /// Tests creating a new file with content
        /// </summary>
        [Fact]
        public void CreateFile_WithContent_CreatesSuccessfully()
        {
            // Arrange
            var content = @"
</rewritten_file> 