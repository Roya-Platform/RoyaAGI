using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace AgentIO.Tests
{
    /// <summary>
    /// Tests that validate the feature files are correctly structured and contain required information.
    /// </summary>
    public class FeatureFileTests
    {
        /// <summary>
        /// Tests that the Introduction.feature file exists and contains the required sections.
        /// </summary>
        [Fact]
        public void IntroductionFeature_ExistsAndContainsRequiredSections()
        {
            // Arrange
            var featureFilePath = Path.Combine("..", "..", "..", "..", "..", "features", "Introduction.feature");
            
            // Act
            var fileExists = File.Exists(featureFilePath);
            var content = fileExists ? File.ReadAllText(featureFilePath) : string.Empty;
            
            // Assert
            fileExists.Should().BeTrue("Introduction.feature file should exist");
            content.Should().Contain("Feature: This is the introduction of the system roles");
            content.Should().Contain("Roles of the system:");
            content.Should().Contain("Kernel:");
            content.Should().Contain("Agent:");
            content.Should().Contain("Node:");
            content.Should().Contain("Firewall:");
            content.Should().Contain("Client:");
            content.Should().Contain("Platform:");
            content.Should().Contain("Engineer:");
            content.Should().Contain("Guard:");
            content.Should().Contain("Here are the terms of the platform:");
            content.Should().Contain("Memory:");
            content.Should().Contain("The goal is client should be able to ask agent for specific actions");
            content.Should().Contain("IO actions:");
            content.Should().Contain("System information:");
            content.Should().Contain("Shell:");
            content.Should().Contain("Startup introduction:");
            content.Should().Contain("Firewall:");
        }

        /// <summary>
        /// Tests that the Introduction.feature file is properly formatted.
        /// </summary>
        [Fact]
        public void IntroductionFeature_IsProperlyFormatted()
        {
            // Arrange
            var featureFilePath = Path.Combine("..", "..", "..", "..", "..", "features", "Introduction.feature");
            
            // Act
            var fileExists = File.Exists(featureFilePath);
            var lines = fileExists ? File.ReadAllLines(featureFilePath) : Array.Empty<string>();
            
            // Assert
            fileExists.Should().BeTrue("Introduction.feature file should exist");
            lines.Should().NotBeEmpty("Feature file should not be empty");
            
            // Check first line starts with "Feature:"
            lines[0].Should().StartWith("Feature:");
            
            // Check for triple quotes for multi-line text
            var containsTripleQuotes = false;
            foreach (var line in lines)
            {
                if (line.Trim() == "\"\"\"")
                {
                    containsTripleQuotes = !containsTripleQuotes;
                }
            }
            
            // Triple quotes should be balanced (open and close)
            containsTripleQuotes.Should().BeFalse("Triple quotes should be balanced in the feature file");
        }
    }
}
