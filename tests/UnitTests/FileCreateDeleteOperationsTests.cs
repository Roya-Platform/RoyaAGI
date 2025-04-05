using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentIO.KernelPlugins;
using AgentIO.Models;
using AgentIO.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AgentIO.Tests
{
    /// <summary>
    /// Tests for file creation and deletion operations
    /// </summary>
    public class FileCreateDeleteOperationsTests : TestBase
    {
        private readonly IOFileWriteOperations _fileWriteOperations;
        private readonly Mock<IFileService> _fileServiceMock;

        public FileCreateDeleteOperationsTests()
        {
            // Get mocks from the service provider
            _fileServiceMock = ServiceProvider.GetRequiredService<Mock<IFileService>>();
            
            // Create the system under test
            _fileWriteOperations = new IOFileWriteOperations(
                ServiceProvider.GetRequiredService<IFileService>(),
                ServiceProvider.GetRequiredService<IAliasService>());
        }

        /// <summary>
        /// Tests creating a new file with content
        /// </summary>
        [Fact]
        public async Task CreateFile_WithContent_CreatesSuccessfully()
        {
            // Arrange
            var filePath = "My Repo/test.txt";
            var content = "This is test content";
            var resolvedPath = "C:/Users/Jeff/repo/test.txt";
            
            _fileServiceMock
                .Setup(fs => fs.WriteFileAsync(resolvedPath, content))
                .ReturnsAsync(true);

            // Act
            var result = await _fileWriteOperations.WriteFileAsync(filePath, content);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("successfully");
            
            // Verify the file service was called with the correct parameters
            _fileServiceMock.Verify(fs => fs.WriteFileAsync(resolvedPath, content), Times.Once);
        }

        /// <summary>
        /// Tests deleting a file
        /// </summary>
        [Fact]
        public async Task DeleteFile_ExistingFile_DeletesSuccessfully()
        {
            // Arrange
            var filePath = "My Repo/test.txt";
            var resolvedPath = "C:/Users/Jeff/repo/test.txt";
            
            _fileServiceMock
                .Setup(fs => fs.DeleteFileAsync(resolvedPath))
                .ReturnsAsync(true);
            
            _fileServiceMock
                .Setup(fs => fs.FileExistsAsync(resolvedPath))
                .ReturnsAsync(true);

            // Act
            var result = await _fileWriteOperations.DeleteFileAsync(filePath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("successfully");
            
            // Verify the file service was called with the correct parameters
            _fileServiceMock.Verify(fs => fs.DeleteFileAsync(resolvedPath), Times.Once);
        }

        /// <summary>
        /// Tests deleting a non-existent file
        /// </summary>
        [Fact]
        public async Task DeleteFile_NonExistentFile_ReturnsError()
        {
            // Arrange
            var filePath = "My Repo/nonexistent.txt";
            var resolvedPath = "C:/Users/Jeff/repo/nonexistent.txt";
            
            _fileServiceMock
                .Setup(fs => fs.FileExistsAsync(resolvedPath))
                .ReturnsAsync(false);

            // Act
            var result = await _fileWriteOperations.DeleteFileAsync(filePath);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("not found");
            
            // Verify the delete method was never called
            _fileServiceMock.Verify(fs => fs.DeleteFileAsync(It.IsAny<string>()), Times.Never);
        }
</rewritten_file> 
